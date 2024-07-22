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

                if(syntaxTree.FilePath.StartsWith(@"D:\Repos\CodeGenerationIvestigation\TestSandBox\obj\Debug\net8.0")) 
                {
                    continue;
                }

                var root = syntaxTree.GetRoot();

                ShowSyntaxNode(0, root);
            }

            // Code generation goes here
        }

        private void ShowSyntaxNode(int n, SyntaxNode syntaxNode)
        {
            FileLogger.WriteLn($"{Spaces(n)}syntaxNode?.Kind() = {syntaxNode?.Kind()}");
            FileLogger.WriteLn($"{Spaces(n)}syntaxNode?.GetText() = {syntaxNode?.GetText()}");

            var childNodes = syntaxNode?.ChildNodes();

            if(childNodes != null)
            {
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