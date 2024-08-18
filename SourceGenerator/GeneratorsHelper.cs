using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using SourceGenerator;

namespace SymOntoClay.SourceGenerator
{
    public static class GeneratorsHelper
    {
        public static string GetPropertyIdentifier(PropertyItem propertyItem)
        {
            return GetPropertyIdentifier(propertyItem.SyntaxNode);
        }

        public static string GetPropertyIdentifier(PropertyDeclarationSyntax syntaxNode)
        {
            return syntaxNode.Identifier.Text;
        }

        public static string GetFieldIdentifier(FieldItem fieldItem)
        {
            return GetFieldIdentifier(fieldItem.SyntaxNode);
        }

        public static string GetFieldIdentifier(FieldDeclarationSyntax syntaxNode)
        {
            var variableDeclarator = syntaxNode.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.VariableDeclaration))?.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.VariableDeclarator)) as VariableDeclaratorSyntax;

            return variableDeclarator.Identifier.Text;
        }

        public static string ToString(SourceText sourceText)
        {
            var sb = new StringBuilder();

            foreach (var line in sourceText.Lines)
            {
                var lineStr = line.ToString();

                if (string.IsNullOrWhiteSpace(lineStr))
                {
                    continue;
                }

                sb.Append(lineStr);
            }

            return sb.ToString().Trim();
        }

        public static void ShowSyntaxNode(int n, SyntaxNode syntaxNode)
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
