using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Arcturus.Validation.CodeGenerator;

[Generator]
public sealed class EndpointValidationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find all invocations that use ValidateParametersFilter
        var endpointsWithValidation = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsValidationFilterInvocation(s),
                transform: static (ctx, _) => GetEndpointParameters(ctx))
            .Where(static m => m is not null);

        // Combine with compilation and generate code
        var compilationAndEndpoints = context.CompilationProvider.Combine(endpointsWithValidation.Collect());

        context.RegisterSourceOutput(compilationAndEndpoints,
            static (spc, source) => Execute(source.Left, source.Right!, spc));
    }

    private static bool IsValidationFilterInvocation(SyntaxNode node)
    {
        // Check for AddEndpointFilter<ValidateParametersFilter>() or ValidateParameters()
        if (node is not InvocationExpressionSyntax invocation)
            return false;

        var methodName = GetMethodName(invocation);
        return methodName is "AddEndpointFilter" or "ValidateParameters";
    }

    private static string? GetMethodName(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression switch
        {
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.Text,
            GenericNameSyntax genericName => genericName.Identifier.Text,
            IdentifierNameSyntax identifierName => identifierName.Identifier.Text,
            _ => null
        };
    }

    private static EndpointParameterInfo? GetEndpointParameters(GeneratorSyntaxContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        // Find the endpoint registration (lambda or method)
        var endpointRegistration = FindEndpointRegistration(invocation);
        if (endpointRegistration is null)
            return null;

        // Extract parameter types from the endpoint
        var parameters = ExtractParameters(endpointRegistration, context.SemanticModel);
        if (parameters.Count == 0)
            return null;

        return new EndpointParameterInfo(parameters);
    }

    private static SyntaxNode? FindEndpointRegistration(InvocationExpressionSyntax validationInvocation)
    {
        // Walk up the tree to find MapGet, MapPost, etc.
        var current = validationInvocation.Parent;
        while (current is not null)
        {
            if (current is InvocationExpressionSyntax invoc)
            {
                var methodName = GetMethodName(invoc);
                if (methodName?.StartsWith("Map") == true)
                {
                    // Found MapGet, MapPost, etc. - get the handler argument
                    if (invoc.ArgumentList.Arguments.Count >= 2)
                    {
                        var handlerArg = invoc.ArgumentList.Arguments[1].Expression;
                        return handlerArg;
                    }
                }
            }
            current = current.Parent;
        }
        return null;
    }

    private static List<ParameterTypeInfo> ExtractParameters(SyntaxNode handler, SemanticModel semanticModel)
    {
        var parameters = new List<ParameterTypeInfo>();

        // Handle lambda expressions
        if (handler is SimpleLambdaExpressionSyntax simpleLambda)
        {
            var paramSymbol = semanticModel.GetDeclaredSymbol(simpleLambda.Parameter);
            if (paramSymbol?.Type is INamedTypeSymbol typeSymbol)
            {
                parameters.Add(new ParameterTypeInfo(
                    paramSymbol.Name,
                    typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    typeSymbol,
                    false));
            }
        }
        else if (handler is ParenthesizedLambdaExpressionSyntax parenLambda)
        {
            foreach (var param in parenLambda.ParameterList.Parameters)
            {
                var paramSymbol = semanticModel.GetDeclaredSymbol(param);
                if (paramSymbol?.Type is INamedTypeSymbol typeSymbol)
                {
                    var hasAsParameters = paramSymbol.GetAttributes()
                        .Any(a => a.AttributeClass?.Name == "AsParametersAttribute");

                    parameters.Add(new ParameterTypeInfo(
                        paramSymbol.Name,
                        typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                        typeSymbol,
                        hasAsParameters));
                }
            }
        }

        return parameters;
    }

    private static void Execute(Compilation compilation, ImmutableArray<EndpointParameterInfo> endpoints, SourceProductionContext context)
    {
        if (endpoints.IsDefaultOrEmpty)
            return;

        // Group all unique parameter types that need validation
        var typesToValidate = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var endpoint in endpoints)
        {
            foreach (var param in endpoint.Parameters)
            {
                typesToValidate.Add(param.TypeSymbol);
            }
        }

        if (typesToValidate.Count == 0)
            return;

        // Generate validation extension class
        var sourceBuilder = new StringBuilder();

        // Determine namespace - use the first compilation assembly's default namespace or global
        var rootNamespace = compilation.AssemblyName ?? "Generated";

        sourceBuilder.AppendLine("// <auto-generated/>");
        sourceBuilder.AppendLine("#nullable enable");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("using System;");
        sourceBuilder.AppendLine("using System.Collections.Generic;");
        sourceBuilder.AppendLine("using System.ComponentModel.DataAnnotations;");
        sourceBuilder.AppendLine("using System.Linq;");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine($"namespace {rootNamespace};");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("internal static partial class ValidationExtensions");
        sourceBuilder.AppendLine("{");

        // Generate validation method for each unique type
        foreach (var typeSymbol in typesToValidate)
        {
            GenerateValidationMethod(sourceBuilder, typeSymbol);
        }

        sourceBuilder.AppendLine("}");

        context.AddSource("ValidationExtensions.g.cs", sourceBuilder.ToString());
    }

    private static void GenerateValidationMethod(StringBuilder sb, INamedTypeSymbol typeSymbol)
    {
        var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var simpleTypeName = typeSymbol.Name;

        sb.AppendLine($"    internal static bool TryValidate(this {typeName} parameters, out Dictionary<string, string[]> errors)");
        sb.AppendLine("    {");
        sb.AppendLine("        errors = new Dictionary<string, string[]>();");
        sb.AppendLine();

        // Use DataAnnotations validation
        sb.AppendLine("        var validationContext = new ValidationContext(parameters);");
        sb.AppendLine("        var validationResults = new List<ValidationResult>();");
        sb.AppendLine();
        sb.AppendLine("        if (!Validator.TryValidateObject(parameters, validationContext, validationResults, validateAllProperties: true))");
        sb.AppendLine("        {");
        sb.AppendLine("            foreach (var validationResult in validationResults)");
        sb.AppendLine("            {");
        sb.AppendLine("                var propertyName = validationResult.MemberNames.FirstOrDefault() ?? \"Unknown\";");
        sb.AppendLine("                errors[propertyName] = new[] { validationResult.ErrorMessage ?? \"Validation failed\" };");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine();

        // Check for required properties on records/classes
        var properties = typeSymbol.GetMembers().OfType<IPropertySymbol>().ToList();
        foreach (var property in properties)
        {
            // Check if property is marked as required (C# 11 feature)
            if (property.IsRequired)
            {
                sb.AppendLine($"        // Check required property: {property.Name}");
                sb.AppendLine($"        if (parameters.{property.Name} == null)");
                sb.AppendLine("        {");
                sb.AppendLine($"            errors[\"{property.Name}\"] = new[] {{ \"The {property.Name} field is required.\" }};");
                sb.AppendLine("        }");
                sb.AppendLine();
            }
        }

        sb.AppendLine("        return errors.Count == 0;");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private class EndpointParameterInfo
    {
        public List<ParameterTypeInfo> Parameters { get; }

        public EndpointParameterInfo(List<ParameterTypeInfo> parameters)
        {
            Parameters = parameters;
        }
    }

    private class ParameterTypeInfo
    {
        public string Name { get; }
        public string TypeName { get; }
        public INamedTypeSymbol TypeSymbol { get; }
        public bool HasAsParameters { get; }

        public ParameterTypeInfo(
            string name,
            string typeName,
            INamedTypeSymbol typeSymbol,
            bool hasAsParameters)
        {
            Name = name;
            TypeName = typeName;
            TypeSymbol = typeSymbol;
            HasAsParameters = hasAsParameters;
        }
    }
}
