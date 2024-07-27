using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SourceGenerator
{
    public class SocSerializationGeneration
    {
        public SocSerializationGeneration(GeneratorExecutionContext context)
        {
            _context = context;
            _pathsList = context.Compilation.SyntaxTrees.Select(t => t.FilePath).ToList();
        }

        private readonly GeneratorExecutionContext _context;
        private readonly List<string> _pathsList;

        public void Run(TargetClassItem targetClassItem)
        {
            FileLogger.WriteLn($"targetClassItem = {targetClassItem}");

            ShowSyntaxNode(0, targetClassItem.SyntaxNode);

            var identationStep = 4;
            var baseIdentation = 0;
            var classDeclIdentation = baseIdentation + identationStep;
            var classContentIdentation = classDeclIdentation + identationStep;

            var plainObjectClassName = GetPlainObjectClassIdentifier(targetClassItem.SyntaxNode);

            FileLogger.WriteLn($"plainObjectClassName = {plainObjectClassName}");

            var sourceCodePlainObjectBuilder = new StringBuilder();

            sourceCodePlainObjectBuilder.AppendLine($"namespace {targetClassItem.Namespace}");
            sourceCodePlainObjectBuilder.AppendLine("{");
            sourceCodePlainObjectBuilder.AppendLine($"{Spaces(classDeclIdentation)}public partial class {plainObjectClassName}");
            sourceCodePlainObjectBuilder.AppendLine($"{Spaces(classDeclIdentation)}{{");
            sourceCodePlainObjectBuilder.AppendLine($"{Spaces(classDeclIdentation)}}}");
            sourceCodePlainObjectBuilder.AppendLine("}");

            FileLogger.WriteLn($"sourceCodePlainObjectBuilder = {sourceCodePlainObjectBuilder}");

            var sourceCodeBuilder = new StringBuilder();
            sourceCodeBuilder.AppendLine("using TestSandBox.Serialization;");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"namespace {targetClassItem.Namespace}");
            sourceCodeBuilder.AppendLine("{");
            sourceCodeBuilder.AppendLine($"{Spaces(classDeclIdentation)}public partial class {GetClassIdentifier(targetClassItem.SyntaxNode)}: ISerializable");
            sourceCodeBuilder.AppendLine($"{Spaces(classDeclIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}Type ISerializable.GetPlainObjectType() => typeof({plainObjectClassName});");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}void ISerializable.OnWritePlainObject(object plainObject, ISerializer serializer)");
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}OnWritePlainObject(({plainObjectClassName})plainObject, serializer);");
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}}}");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}void OnWritePlainObject({plainObjectClassName} plainObject, ISerializer serializer)");
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}}}");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}void ISerializable.OnReadPlainObject(object plainObject, IDeserializer deserializer)");
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}OnReadPlainObject(({plainObjectClassName})plainObject, deserializer);");
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}}}");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}void OnReadPlainObject({plainObjectClassName} plainObject, IDeserializer deserializer)");
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{Spaces(classContentIdentation)}}}");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{Spaces(classDeclIdentation)}}}");
            sourceCodeBuilder.AppendLine("}");

            FileLogger.WriteLn($"sourceCodeBuilder = {sourceCodeBuilder}");

            var fileName = $"{targetClassItem.Namespace}.{targetClassItem.Identifier}.g.cs";

            FileLogger.WriteLn($"fileName = {fileName}");

            SaveFile(sourceCodeBuilder.ToString(), fileName);

            var plainObjectFileName = $"{targetClassItem.Namespace}.{targetClassItem.Identifier}Po.g.cs";

            FileLogger.WriteLn($"plainObjectFileName = {plainObjectFileName}");

            SaveFile(sourceCodePlainObjectBuilder.ToString(), plainObjectFileName);
        }

        private string GetClassIdentifier(ClassDeclarationSyntax syntaxNode)
        {
            var sb = new StringBuilder(syntaxNode.Identifier.Text);

            return sb.ToString();
        }

        private string GetPlainObjectClassIdentifier(ClassDeclarationSyntax syntaxNode)
        {
            var sb = new StringBuilder(syntaxNode.Identifier.Text);
            sb.Append("Po");
            return sb.ToString();
        }

        private void SaveFile(string source, string fileName)
        {
            var fileNameForSearch = $"\\{fileName}";

            FileLogger.WriteLn($"fileNameForSearch = {fileNameForSearch}");

            if (_pathsList.Any(p => p.EndsWith(fileNameForSearch)))
            {
                var path = _pathsList.First(p => p.EndsWith(fileNameForSearch));

                FileLogger.WriteLn($"path = {path}");

                File.WriteAllText(path, source, Encoding.UTF8);
            }
            else
            {
                _context.AddSource(fileName, SourceText.From(source, Encoding.UTF8));
            }
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
