using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;
using System.Text;

namespace SourceGenerator
{
    [Generator]
    public class HelloSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            FileLogger.WriteLn("Hi");

            var syntaxTrees = context.Compilation.SyntaxTrees;

            foreach(var syntaxTree in syntaxTrees)
            {
                FileLogger.WriteLn($"syntaxTree.FilePath = {syntaxTree.FilePath}");

                var root = syntaxTree.GetRoot();

                ShowSyntaxNode(0, root);
            }

            FileLogger.WriteLn("-----------------");

            var searcher = new TargetClassSearcher(syntaxTrees);

            var items = searcher.Run("SocSerialization");

            FileLogger.WriteLn($"items.Count = {items.Count}");

            foreach(var item in items)
            {
                FileLogger.WriteLn($"item = {item}");
                ShowSyntaxNode(0, item.SyntaxNode);
            }

            // Code generation goes here

            FileLogger.WriteLn("||||||||||||||||||||||||||");

            var source = @"
public class tmpFile1223434325
{
}";

            context.AddSource("tmpFile.g.cs", source);

            FileLogger.WriteLn("|---------------|");
        }

        private void ShowSyntaxNode(int n, SyntaxNode syntaxNode)
        {
            FileLogger.WriteLn($"{Spaces(n)}syntaxNode?.GetType().Name = {syntaxNode?.GetType().Name}");
            FileLogger.WriteLn($"{Spaces(n)}syntaxNode?.Kind() = {syntaxNode?.Kind()}");
            FileLogger.WriteLn($"{Spaces(n)}syntaxNode?.GetText() = {syntaxNode?.GetText()}");

            var childNodes = syntaxNode?.ChildNodes();

            FileLogger.WriteLn($"{Spaces(n)}childNodes = {childNodes == null}");

            if (childNodes != null)
            {
                FileLogger.WriteLn($"{Spaces(n)}childNodes.Count() = {childNodes.Count()}");

                foreach (var childNode in childNodes)
                {
                    ShowSyntaxNode(n + 4, childNode);
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }

        public static string Spaces(int n)
        {
            if (n == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            for (var i = 0; i < n; i++)
            {
                sb.Append(" ");
            }

            return sb.ToString();
        }
    }
}
//<EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>