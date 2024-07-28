using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace SourceGenerator
{
    public class FieldItem: BaseFieldItem
    {
        public FieldDeclarationSyntax SyntaxNode { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{nameof(KindFieldType)} = {KindFieldType}");
            //sb.AppendLine($"{nameof()} = {}");
            return sb.ToString();
        }
    }
}
