using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceGenerator
{
    public class TargetClassSearcher
    {
        public TargetClassSearcher(IEnumerable<SyntaxTree> syntaxTrees) 
        {
            _syntaxTrees = syntaxTrees;
        }

        private readonly IEnumerable<SyntaxTree> _syntaxTrees;

        public List<TargetClassItem> Run(string attributeName)
        {
            return Run(new List<string> { attributeName });
        }

        public List<TargetClassItem> Run(List<string> attributeNames)
        {
#if DEBUG
            FileLogger.WriteLn($"attributeNames.Count = {attributeNames.Count}");
            foreach (var attributeName in attributeNames)
            {
                FileLogger.WriteLn($"attributeName = {attributeName}");
            }
#endif

            var result = new List<TargetClassItem>();

            var context = new TargetClassSearcherContext();

            foreach (var syntaxTree in _syntaxTrees)
            {
                if(syntaxTree.FilePath.EndsWith(".g.cs"))
                {
                    continue;
                }

                ProcessSyntaxTree(syntaxTree, attributeNames, context, ref result);
            }

            return result;
        }

        private void ProcessSyntaxTree(SyntaxTree syntaxTree, List<string> attributeNames, TargetClassSearcherContext context, ref List<TargetClassItem> result)
        {
#if DEBUG
            FileLogger.WriteLn($"syntaxTree.FilePath = {syntaxTree.FilePath}");
#endif

            var root = syntaxTree.GetRoot();

            FileLogger.WriteLn($"root?.GetKind() = {root?.Kind()}");
            FileLogger.WriteLn($"root?.GetText() = {root?.GetText()}");

            var childNodes = root?.ChildNodes();

            if(childNodes == null)
            {
                return;
            }

            var namespaceDeclarations = childNodes.Where(p => p.IsKind(SyntaxKind.NamespaceDeclaration));

#if DEBUG
            FileLogger.WriteLn($"namespaceDeclarations.Count() = {namespaceDeclarations.Count()}");
#endif

            if(namespaceDeclarations.Count() == 0)
            {
                return;
            }

            context.FilePath = syntaxTree.FilePath;

#if DEBUG
            FileLogger.WriteLn($"context = {context}");
#endif

            foreach (var namespaceDeclaration in namespaceDeclarations)
            {
                ProcessNamespaceDeclaration(namespaceDeclaration, attributeNames, context, ref result);
            }
        }

        private void ProcessNamespaceDeclaration(SyntaxNode namespaceDeclaration, List<string> attributeNames, TargetClassSearcherContext context, ref List<TargetClassItem> result)
        {
#if DEBUG
            FileLogger.WriteLn($"namespaceDeclaration?.GetKind() = {namespaceDeclaration?.Kind()}");
            FileLogger.WriteLn($"namespaceDeclaration?.GetText() = {namespaceDeclaration?.GetText()}");
#endif

            var childNodes = namespaceDeclaration?.ChildNodes();

            var classDeclarations = childNodes.Where(p => p.IsKind(SyntaxKind.ClassDeclaration));

#if DEBUG
            FileLogger.WriteLn($"classDeclarations.Count() = {classDeclarations.Count()}");
#endif

            if (classDeclarations.Count() == 0)
            {
                return;
            }

            var namespaceIdentifierNode = childNodes.Single(p => p.IsKind(SyntaxKind.QualifiedName) || p.IsKind(SyntaxKind.IdentifierName));

#if DEBUG
            FileLogger.WriteLn($"namespaceIdentifierNode?.GetKind() = {namespaceIdentifierNode?.Kind()}");
            FileLogger.WriteLn($"namespaceIdentifierNode?.GetText() = {namespaceIdentifierNode?.GetText()}");
#endif

            var namespaceIdentifier = ToString(namespaceIdentifierNode?.GetText());

#if DEBUG
            FileLogger.WriteLn($"namespaceIdentifier = '{namespaceIdentifier}'");
#endif

            context.Namespace = namespaceIdentifier;

#if DEBUG
            FileLogger.WriteLn($"context = {context}");
#endif

            foreach(var classDeclaration in classDeclarations)
            {
                ProcessClassDeclaration(classDeclaration, attributeNames, context, ref result);
            }
        }

        private void ProcessClassDeclaration(SyntaxNode classDeclaration, List<string> attributeNames, TargetClassSearcherContext context, ref List<TargetClassItem> result)
        {
#if DEBUG
            FileLogger.WriteLn($"classDeclaration?.GetKind() = {classDeclaration?.Kind()}");
            FileLogger.WriteLn($"classDeclaration?.GetText() = {classDeclaration?.GetText()}");
#endif

            var childNodes = classDeclaration?.ChildNodes();

            var attributesList = childNodes
                .Where(p => p.IsKind(SyntaxKind.AttributeList))
                .SelectMany(p => p.ChildNodes().Where(x => x.IsKind(SyntaxKind.Attribute)).SelectMany(y => y.ChildNodes().Where(u => u.IsKind(SyntaxKind.IdentifierName))))
                .Select(p => ToString(p.GetText()));

#if DEBUG
            FileLogger.WriteLn($"attributesList.Count() = {attributesList.Count()}");
            foreach (var attribute in attributesList)
            {
                FileLogger.WriteLn($"attribute = '{attribute}'");
                //ShowSyntaxNode(0, attribute);
            }
#endif

            if(!attributesList.Any(p => attributeNames.Contains(p)))
            {
                return;
            }

            var cSharpClassDeclarationSyntax = (ClassDeclarationSyntax)classDeclaration;

#if DEBUG
            FileLogger.WriteLn($"cSharpClassDeclarationSyntax.Identifier = {cSharpClassDeclarationSyntax.Identifier}");
#endif

            var resultItem = new TargetClassItem
            {
                FilePath = context.FilePath,
                Namespace = context.Namespace,
                Identifier = cSharpClassDeclarationSyntax.Identifier.ToString(),
                SyntaxNode = cSharpClassDeclarationSyntax
            };

#if DEBUG
            FileLogger.WriteLn($"resultItem = {resultItem}");
#endif

            result.Add(resultItem);
        }

        private string ToString(SourceText sourceText)
        {
            var sb = new StringBuilder();

            foreach (var line in sourceText.Lines)
            { 
                var lineStr = line.ToString();

                if(string.IsNullOrWhiteSpace(lineStr))
                {
                    continue;
                }

                sb.Append(lineStr);
            }

            return sb.ToString();
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
