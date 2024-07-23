using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

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

            foreach (var syntaxTree in _syntaxTrees)
            {
                ProcessSyntaxTree(syntaxTree, ref result);
            }

            return result;
        }

        private void ProcessSyntaxTree(SyntaxTree syntaxTree, ref List<CustomSerializationItem> result)
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

            var context = new CustomSerializationSearcherContext()
            {
                FilePath = syntaxTree.FilePath
            };

#if DEBUG
            FileLogger.WriteLn($"context = {context}");
#endif
        }
    }
}
