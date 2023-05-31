using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;
using System.Text;

namespace TelemetryProxy.Gen;

[Generator]
public class TestSourceGen : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: FilterIServiceImplementations,
            transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node
            );

        context.RegisterSourceOutput(classes,
            static (ctx, source) => Execute(ctx, source));
    }

    private static bool FilterIServiceImplementations(SyntaxNode node, CancellationToken arg2)
    {
        if (node is not ClassDeclarationSyntax @class)
            return false;

        if (@class.BaseList is null)
            return false;

        return @class.BaseList.Types.Any(t => t.Type.GetText().ToString().Trim('\r', '\n') == "IServiceTracer");
    }

    private static void Execute(SourceProductionContext ctx, ClassDeclarationSyntax classDeclarationSyntax)
    {
        var namespaceName = GetNamespace(classDeclarationSyntax);
        var className = classDeclarationSyntax.Identifier.Text;
        var fileName = $"{namespaceName}.{className}.g.cs";

        var bld = new StringBuilder();

        bld.Append(@$"
namespace {namespaceName}{{
    public static class {className}Extensions {{
        public static OpenTelemetry.Trace.TracerProviderBuilder Add{className}(this OpenTelemetry.Trace.TracerProviderBuilder bld) {{
            return bld.AddSource({namespaceName}.{className}.TracerName);
        }}
    }} 
}}");

        ctx.AddSource(fileName, bld.ToString());
    }

    private static string GetNamespace(ClassDeclarationSyntax @class)
    {
        return @class.Parent switch
        {
            FileScopedNamespaceDeclarationSyntax @namespace => @namespace.Name.ToString(),
            NamespaceDeclarationSyntax @namespace => @namespace.Name.ToString(),
            ClassDeclarationSyntax parentClass => $"{GetNamespace(parentClass)}.{parentClass.Identifier.Text}",
            _ => "Nope"
        };
    }
}