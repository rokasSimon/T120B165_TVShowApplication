using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace StaticCodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StaticCodeAnalysisAnalyzer : DiagnosticAnalyzer
    {
        public const string ReadonlyFieldRuleId = "Custom1";
        public const string RequiredBracesRuleId = "Custom2";
        public const string LiteralRuleId = "Custom3";

        private static readonly LocalizableString _readonlyFieldRuleTitle = new LocalizableResourceString(nameof(Resources.ReadonlyFieldTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _readonlyFieldRuleMessageFormat = new LocalizableResourceString(nameof(Resources.ReadonlyFieldRuleFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _readonlyFieldRuleDescription = new LocalizableResourceString(nameof(Resources.ReadonlyFieldRuleDescription), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString _requiredBracesRuleTitle = new LocalizableResourceString(nameof(Resources.RequiredBracesRuleTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _requiredBracesRuleMessageFormat = new LocalizableResourceString(nameof(Resources.RequiredBracesRuleFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _requiredBracesRuleDescription = new LocalizableResourceString(nameof(Resources.RequiredBracesRuleDescription), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString _literalRuleTitle = new LocalizableResourceString(nameof(Resources.LiteralRuleTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _literalRuleMessageFormat = new LocalizableResourceString(nameof(Resources.LiteralRuleFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _literalRuleDescription = new LocalizableResourceString(nameof(Resources.LiteralRuleDescription), Resources.ResourceManager, typeof(Resources));

        private const string NamingCategory = "Naming";
        private const string SyntaxCategory = "Syntax";

        private static readonly DiagnosticDescriptor _readonlyFieldRule =
            new DiagnosticDescriptor(ReadonlyFieldRuleId, _readonlyFieldRuleTitle, _readonlyFieldRuleMessageFormat, NamingCategory, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _readonlyFieldRuleDescription);
        private static readonly DiagnosticDescriptor _requiredBracesRule =
            new DiagnosticDescriptor(RequiredBracesRuleId, _requiredBracesRuleTitle, _requiredBracesRuleMessageFormat, SyntaxCategory, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _requiredBracesRuleDescription);
        private static readonly DiagnosticDescriptor _literalRule =
            new DiagnosticDescriptor(LiteralRuleId, _literalRuleTitle, _literalRuleMessageFormat, SyntaxCategory, DiagnosticSeverity.Info, isEnabledByDefault: true, description: _literalRuleDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        { 
            get
            {
                return ImmutableArray.Create(_readonlyFieldRule, _requiredBracesRule, _literalRule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeFields, SymbolKind.Field);
            context.RegisterSyntaxTreeAction(AnalyzeStatements);
            context.RegisterSyntaxNodeAction(AnalyzeStringLiterals, SyntaxKind.StringLiteralExpression);
        }

        private static void AnalyzeFields(SymbolAnalysisContext context)
        {
            var namedTypeSymbol = (IFieldSymbol)context.Symbol;

            if (namedTypeSymbol.IsReadOnly && namedTypeSymbol.DeclaredAccessibility == Accessibility.Private)
            {
                var fieldName = namedTypeSymbol.Name;

                if (fieldName.Length > 1
                    && fieldName[0] == '_'
                    && char.IsLower(fieldName[1]))
                {
                    return;
                }
                else
                {
                    var diagnostic = Diagnostic.Create(_readonlyFieldRule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static void AnalyzeStatements(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);
            foreach (var statement in root.DescendantNodes().OfType<IfStatementSyntax>())
            {
                var executionBlock = statement.Statement;

                if (executionBlock is BlockSyntax
                    || executionBlock is ReturnStatementSyntax
                    || executionBlock is ThrowStatementSyntax)
                {
                    continue;
                }

                var diagnostic = Diagnostic.Create(_requiredBracesRule, executionBlock.GetLocation());

                context.ReportDiagnostic(diagnostic);
            }
        }

        private static void AnalyzeStringLiterals(SyntaxNodeAnalysisContext context)
        {
            var literalNode = context.Node as LiteralExpressionSyntax;

            if (literalNode.Parent is ArgumentSyntax)
            {
                var diagnostic = Diagnostic.Create(_literalRule, literalNode.GetLocation(), literalNode.ToString());

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
