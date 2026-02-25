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
        // Check for AddEndpointFilter<ValidateParametersFilter>() or ValidateParameters() on an endpoint
        if (node is not InvocationExpressionSyntax invocation)
            return false;

        var methodName = GetMethodName(invocation);
        if (methodName is null)
            return false;

        // Match AddEndpointFilter<ValidateParametersFilter>()
        if (methodName == "AddEndpointFilter")
        {
            // The expression can be a generic invocation directly or via a member access, e.g. endpoints.AddEndpointFilter<...>()
            GenericNameSyntax? genericName = invocation.Expression as GenericNameSyntax;

            if (genericName is null &&
                invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name is GenericNameSyntax memberGeneric)
            {
                genericName = memberGeneric;
            }

            if (genericName is null)
                return false;

            var typeArguments = genericName.TypeArgumentList.Arguments;
            if (typeArguments.Count != 1)
                return false;

            if (typeArguments[0] is IdentifierNameSyntax typeIdentifier &&
                typeIdentifier.Identifier.Text == "ValidateParametersFilter")
            {
                return true;
            }

            return false;
        }

        // Match ValidateParameters() chained on minimal API endpoint registrations,
        // e.g., app.MapGet(...).ValidateParameters();
        if (methodName == "ValidateParameters" &&
            invocation.Expression is MemberAccessExpressionSyntax validateMember &&
            validateMember.Expression is InvocationExpressionSyntax endpointInvocation)
        {
            var endpointMethodName = GetMethodName(endpointInvocation);
            return endpointMethodName is "MapGet" or "MapPost" or "MapPut" or "MapDelete" or "MapMethods";
        }

        return false;
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
        // For .AddEndpointFilter<ValidateParametersFilter>() or .ValidateParameters()
        // The pattern is: app.MapGet(...).AddEndpointFilter<...>()
        // We need to find the MapGet/MapPost/etc. invocation

        // Check if this is a chained call (member access)
        if (validationInvocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            // The expression part should be the previous call in the chain
            var previousExpression = memberAccess.Expression;

            // Keep traversing the chain to find the Map* method
            while (previousExpression != null)
            {
                if (previousExpression is InvocationExpressionSyntax invoc)
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

                    // Check if this invocation is also a chain
                    if (invoc.Expression is MemberAccessExpressionSyntax nextMember)
                    {
                        previousExpression = nextMember.Expression;
                        continue;
                    }
                }

                // Try member access
                if (previousExpression is MemberAccessExpressionSyntax ma)
                {
                    previousExpression = ma.Expression;
                    continue;
                }

                break;
            }
        }

        // Fallback: Walk up the tree to find MapGet, MapPost, etc.
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
        {
            // Generate a diagnostic comment file to help debug
            context.AddSource("ValidationDiagnostics.g.cs", 
                "// No endpoints with validation found. Make sure you're using .ValidateParameters() or .AddEndpointFilter<ValidateParametersFilter>()");
            return;
        }

        // Group all unique parameter types that need validation
        var typesToValidate = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        var diagnosticInfo = new StringBuilder();
        diagnosticInfo.AppendLine("// Validation Generator Diagnostics:");
        diagnosticInfo.AppendLine($"// Found {endpoints.Length} endpoint(s) with validation");

        foreach (var endpoint in endpoints)
        {
            diagnosticInfo.AppendLine($"// Endpoint with {endpoint.Parameters.Count} parameter(s):");
            foreach (var param in endpoint.Parameters)
            {
                var isPrimitive = IsPrimitiveOrFrameworkType(param.TypeSymbol);
                var hasValidationAttributes = HasValidationAttributes(param.TypeSymbol);

                diagnosticInfo.AppendLine($"//   - {param.Name}: {param.TypeName}");
                diagnosticInfo.AppendLine($"//     IsPrimitive: {isPrimitive}, HasValidation: {hasValidationAttributes}, AsParameters: {param.HasAsParameters}");

                // Include types that either have validation attributes OR are complex types (not primitives/framework types)
                if (!isPrimitive)
                {
                    typesToValidate.Add(param.TypeSymbol);
                    diagnosticInfo.AppendLine($"//     ✓ Added to validation list");
                }
                else
                {
                    diagnosticInfo.AppendLine($"//     ✗ Skipped (primitive/framework type)");
                }
            }
        }

        diagnosticInfo.AppendLine($"// Total types to validate: {typesToValidate.Count}");

        if (typesToValidate.Count == 0)
        {
            context.AddSource("ValidationDiagnostics.g.cs", diagnosticInfo.ToString());
            return;
        }

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
        sourceBuilder.Append(diagnosticInfo);
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

        // Generate the partial ValidateParametersFilter implementation
        GenerateValidateParametersFilter(compilation, typesToValidate, context);
    }

    private static bool HasValidationAttributes(ITypeSymbol typeSymbol)
    {
        // Check if the type or any of its properties have validation attributes
        if (typeSymbol is not INamedTypeSymbol namedType)
            return false;

        foreach (var member in namedType.GetMembers())
        {
            if (member is IPropertySymbol property)
            {
                if (property.IsRequired)
                    return true;

                foreach (var attribute in property.GetAttributes())
                {
                    var attrName = attribute.AttributeClass?.Name;
                    if (attrName != null && (
                        attrName.EndsWith("Attribute") && 
                        attribute.AttributeClass?.ContainingNamespace?.ToDisplayString() == "System.ComponentModel.DataAnnotations"))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private static void GenerateValidateParametersFilter(Compilation compilation, HashSet<INamedTypeSymbol> typesToValidate, SourceProductionContext context)
    {
        var sourceBuilder = new StringBuilder();

        // Use the same namespace as ValidationExtensions
        var rootNamespace = compilation.AssemblyName ?? "Generated";

        sourceBuilder.AppendLine("// <auto-generated/>");
        sourceBuilder.AppendLine("#nullable enable");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("using Microsoft.AspNetCore.Http;");
        sourceBuilder.AppendLine("using System;");
        sourceBuilder.AppendLine("using System.Collections.Generic;");
        sourceBuilder.AppendLine("using System.Threading.Tasks;");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine($"namespace {rootNamespace};");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("// Generated filter implementation with compile-time validation");
        sourceBuilder.AppendLine("file sealed class GeneratedValidateParametersFilter : Arcturus.Validation.ValidateParametersFilter");
        sourceBuilder.AppendLine("{");
        sourceBuilder.AppendLine("    public override async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)");
        sourceBuilder.AppendLine("    {");
        sourceBuilder.AppendLine("        // only capture bad request responses ");
        sourceBuilder.AppendLine("        if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)");
        sourceBuilder.AppendLine("        {");
        sourceBuilder.AppendLine("            // Validate arguments");
        sourceBuilder.AppendLine("            foreach (var argument in context.Arguments)");
        sourceBuilder.AppendLine("            {");
        sourceBuilder.AppendLine("                if (argument is null)");
        sourceBuilder.AppendLine("                    continue;");
        sourceBuilder.AppendLine();

        // Generate type checks for each validatable type
        var typesList = typesToValidate.ToList();
        for (int i = 0; i < typesList.Count; i++)
        {
            var typeSymbol = typesList[i];
            var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var ifKeyword = i == 0 ? "if" : "else if";

            sourceBuilder.AppendLine($"                {ifKeyword} (argument is {typeName} param{i})");
            sourceBuilder.AppendLine("                {");
            sourceBuilder.AppendLine($"                    if (!param{i}.TryValidate(out var errors{i}))");
            sourceBuilder.AppendLine("                    {");
            sourceBuilder.AppendLine($"                        return Results.ValidationProblem(errors{i});");
            sourceBuilder.AppendLine("                    }");
            sourceBuilder.AppendLine("                }");
        }

        sourceBuilder.AppendLine("            }");
        sourceBuilder.AppendLine("        }");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("        return await next(context);");
        sourceBuilder.AppendLine("    }");
        sourceBuilder.AppendLine("}");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("// Module initializer to register the generated filter");
        sourceBuilder.AppendLine("file static class ValidateParametersFilterRegistration");
        sourceBuilder.AppendLine("{");
        sourceBuilder.AppendLine("    [System.Runtime.CompilerServices.ModuleInitializer]");
        sourceBuilder.AppendLine("    internal static void Initialize()");
        sourceBuilder.AppendLine("    {");
        sourceBuilder.AppendLine("        Arcturus.Validation.ValidateParametersFilterFactory.SetFactory(() => new GeneratedValidateParametersFilter());");
        sourceBuilder.AppendLine("    }");
        sourceBuilder.AppendLine("}");

        context.AddSource("GeneratedValidateParametersFilter.g.cs", sourceBuilder.ToString());
    }

    private static bool IsPrimitiveOrFrameworkType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.TypeKind == TypeKind.Enum)
            return true;

        var fullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return typeSymbol.SpecialType != SpecialType.None
            || fullName.StartsWith("global::System.String")
            || fullName.StartsWith("global::System.DateTime")
            || fullName.StartsWith("global::System.DateTimeOffset")
            || fullName.StartsWith("global::System.TimeSpan")
            || fullName.StartsWith("global::System.Guid")
            || fullName.StartsWith("global::System.Decimal")
            || fullName.StartsWith("global::Microsoft.")
            || (fullName.StartsWith("global::System.") && !fullName.StartsWith("global::System.ComponentModel.DataAnnotations"));
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
