using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceGenerator
{
    public class CustomSerializationSearcher
    {
        public CustomSerializationSearcher(IEnumerable<SyntaxTree> syntaxTrees) 
        {
            _syntaxTrees = syntaxTrees;
        }

        private readonly IEnumerable<SyntaxTree> _syntaxTrees;

        public List<CustomSerializationItem> Run()
        {
            var result = new List<CustomSerializationItem>();

            var context = new CustomSerializationSearcherContext();

            foreach (var syntaxTree in _syntaxTrees)
            {
                ProcessSyntaxTree(syntaxTree, context, ref result);
            }

            return result;
        }

        private void ProcessSyntaxTree(SyntaxTree syntaxTree, CustomSerializationSearcherContext context, ref List<CustomSerializationItem> result)
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
                ProcessNamespaceDeclaration(namespaceDeclaration, context, ref result);
            }
        }

        private void ProcessNamespaceDeclaration(SyntaxNode namespaceDeclaration, CustomSerializationSearcherContext context, ref List<CustomSerializationItem> result)
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
                ProcessClassDeclaration(classDeclaration, context, ref result);
            }
        }

        private void ProcessClassDeclaration(SyntaxNode classDeclaration, CustomSerializationSearcherContext context, ref List<CustomSerializationItem> result)
        {
#if DEBUG
            FileLogger.WriteLn($"classDeclaration?.GetKind() = {classDeclaration?.Kind()}");
            FileLogger.WriteLn($"classDeclaration?.GetText() = {classDeclaration?.GetText()}");
#endif
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
    }
}
