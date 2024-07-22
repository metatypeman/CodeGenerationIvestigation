using Microsoft.CodeAnalysis;
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


    }
}
