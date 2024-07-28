using Microsoft.CodeAnalysis;

namespace SourceGenerator
{
    public abstract class BaseFieldItem
    {
        public KindFieldType KindFieldType { get; set; } = KindFieldType.Unknown;
        public SyntaxNode FieldTypeSyntaxNode { get; set; }
    }
}
