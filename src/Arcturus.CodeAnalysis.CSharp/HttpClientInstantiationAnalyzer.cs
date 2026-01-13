using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Arcturus.CodeAnalysis.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class HttpClientInstantiationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "SDA001";
    private static readonly LocalizableString _title = "Avoid direct HttpClient instantiation";
    private static readonly LocalizableString _messageFormat = "Directly creating HttpClient can lead to socket exhaustion. Use IHttpClientFactory or a shared instance instead.";
    private static readonly LocalizableString _description = "Detects 'new HttpClient()' or 'using var x = new HttpClient();'.";
    private const string _category = "Usage";

    private static readonly DiagnosticDescriptor _rule = new (
        DiagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Warning,
        isEnabledByDefault: true, description: _description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
    }

    private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

        var typeInfo = context.SemanticModel.GetTypeInfo(objectCreation);
        if (typeInfo.Type?.ToDisplayString() == "System.Net.Http.HttpClient")
        {
            var diagnostic = Diagnostic.Create(_rule, objectCreation.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
