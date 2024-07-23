using System.Text;

namespace SourceGenerator
{
    public class CustomSerializationSearcherContext
    {
        public string FilePath { get; set; }
        public string Namespace { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{nameof(FilePath)} = {FilePath}");
            sb.AppendLine($"{nameof(Namespace)} = {Namespace}");
            return sb.ToString();
        }
    }
}
