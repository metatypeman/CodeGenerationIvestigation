using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

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

            var context = new CustomSerializationSearcherContext()
            {
                FilePath = syntaxTree.FilePath
            };

            var root = syntaxTree.GetRoot();

            FileLogger.WriteLn($"root?.GetKind() = {root?.Kind()}");
            FileLogger.WriteLn($"root?.GetText() = {root?.GetText()}");

#if DEBUG
            FileLogger.WriteLn($"context = {context}");
#endif
        }
    }
}
