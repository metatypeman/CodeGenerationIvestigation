using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
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
#if DEBUG
            FileLogger.WriteLn($"targetClassItem = {targetClassItem}");
            //ShowSyntaxNode(0, targetClassItem.SyntaxNode);
#endif

            var identationStep = 4;
            var baseIdentation = 0;
            var classDeclIdentation = baseIdentation + identationStep;
            var classContentIdentation = classDeclIdentation + identationStep;

            var plainObjectClassName = GetPlainObjectClassIdentifier(targetClassItem.SyntaxNode);

#if DEBUG
            FileLogger.WriteLn($"plainObjectClassName = {plainObjectClassName}");
#endif

            var propertyItems = GetPropertyItems(targetClassItem.SyntaxNode);

#if DEBUG
            FileLogger.WriteLn($"propertyItems.Count = {propertyItems.Count}");
#endif

            var fieldsItems = GetFieldItems(targetClassItem.SyntaxNode);

#if DEBUG
            FileLogger.WriteLn($"fieldsItems.Count = {fieldsItems.Count}");
#endif

            var sourceCodePlainObjectBuilder = new StringBuilder();

            sourceCodePlainObjectBuilder.AppendLine($"namespace {targetClassItem.Namespace}");
            sourceCodePlainObjectBuilder.AppendLine("{");
            sourceCodePlainObjectBuilder.AppendLine($"{GeneratorsHelper.Spaces(classDeclIdentation)}public partial class {plainObjectClassName}");
            sourceCodePlainObjectBuilder.AppendLine($"{GeneratorsHelper.Spaces(classDeclIdentation)}{{");
            sourceCodePlainObjectBuilder.AppendLine($"{GeneratorsHelper.Spaces(classDeclIdentation)}}}");
            sourceCodePlainObjectBuilder.AppendLine("}");

#if DEBUG
            FileLogger.WriteLn($"sourceCodePlainObjectBuilder = {sourceCodePlainObjectBuilder}");
#endif

            var sourceCodeBuilder = new StringBuilder();
            sourceCodeBuilder.AppendLine("using TestSandBox.Serialization;");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"namespace {targetClassItem.Namespace}");
            sourceCodeBuilder.AppendLine("{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classDeclIdentation)}public partial class {GetClassIdentifier(targetClassItem.SyntaxNode)}: ISerializable");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classDeclIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}Type ISerializable.GetPlainObjectType() => typeof({plainObjectClassName});");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}void ISerializable.OnWritePlainObject(object plainObject, ISerializer serializer)");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}OnWritePlainObject(({plainObjectClassName})plainObject, serializer);");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}}}");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}void OnWritePlainObject({plainObjectClassName} plainObject, ISerializer serializer)");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}}}");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}void ISerializable.OnReadPlainObject(object plainObject, IDeserializer deserializer)");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}OnReadPlainObject(({plainObjectClassName})plainObject, deserializer);");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}}}");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}void OnReadPlainObject({plainObjectClassName} plainObject, IDeserializer deserializer)");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}}}");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classDeclIdentation)}}}");
            sourceCodeBuilder.AppendLine("}");

#if DEBUG
            FileLogger.WriteLn($"sourceCodeBuilder = {sourceCodeBuilder}");
#endif

            var fileName = $"{targetClassItem.Namespace}.{targetClassItem.Identifier}.g.cs";

#if DEBUG
            FileLogger.WriteLn($"fileName = {fileName}");
#endif

            SaveFile(sourceCodeBuilder.ToString(), fileName);

            var plainObjectFileName = $"{targetClassItem.Namespace}.{targetClassItem.Identifier}Po.g.cs";

#if DEBUG
            FileLogger.WriteLn($"plainObjectFileName = {plainObjectFileName}");
#endif

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

        private List<PropertyItem> GetPropertyItems(ClassDeclarationSyntax syntaxNode)
        {
            var result = new List<PropertyItem>();

            var propertiesDeclarationsList = syntaxNode.ChildNodes()?.Where(p => p.IsKind(SyntaxKind.PropertyDeclaration) && IsSerializedProperty(p)) ?? new List<SyntaxNode>();

#if DEBUG
            //FileLogger.WriteLn($"propertiesDeclarationsList.Count() = {propertiesDeclarationsList.Count()}");
#endif

            if(propertiesDeclarationsList.Count() == 0)
            {
                return result;
            }

            foreach(var propertyDeclaration in propertiesDeclarationsList)
            {
#if DEBUG
                //GeneratorsHelper.ShowSyntaxNode(0, propertyDeclaration);
#endif

                var propertyDeclarationSyntax = (PropertyDeclarationSyntax)propertyDeclaration;

                var item = new PropertyItem()
                {
                    ClassDeclarationSyntaxNode = syntaxNode,
                    SyntaxNode = propertyDeclarationSyntax
                };

                FillUpBaseFieldItem(propertyDeclaration, item);

#if DEBUG
                //FileLogger.WriteLn($"item = {item}");
#endif

                result.Add(item);
            }

            return result;
        }

        private static bool IsSerializedProperty(SyntaxNode propertyDeclarationNode)
        {
#if DEBUG
            //GeneratorsHelper.ShowSyntaxNode(0, propertyDeclarationNode);
#endif

            var accessorList = propertyDeclarationNode.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.AccessorList));

#if DEBUG
            //GeneratorsHelper.ShowSyntaxNode(0, accessorList);
#endif

            if (accessorList == null)
            {
                return false;
            }

            var getAccessorDeclaration = accessorList.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.GetAccessorDeclaration));

#if DEBUG
            //GeneratorsHelper.ShowSyntaxNode(0, getAccessorDeclaration);
#endif

            if ((getAccessorDeclaration?.ChildNodes()?.Count() ?? 0) > 0)
            {
                return false;
            }

            var setAccessorDeclaration = accessorList.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.SetAccessorDeclaration));

#if DEBUG
            //GeneratorsHelper.ShowSyntaxNode(0, setAccessorDeclaration);
#endif

            if ((setAccessorDeclaration?.ChildNodes()?.Count() ?? 0) > 0)
            {
                return false;
            }

            return true;
        }

        private List<FieldItem> GetFieldItems(ClassDeclarationSyntax syntaxNode)
        {
            var result = new List<FieldItem>();

            var fieldsDeclarationList = syntaxNode.ChildNodes()?.Where(p => p.IsKind(SyntaxKind.FieldDeclaration)) ?? new List<SyntaxNode>();

#if DEBUG
            //FileLogger.WriteLn($"fieldsDeclarationList.Count() = {fieldsDeclarationList.Count()}");
#endif

            if (fieldsDeclarationList.Count() == 0)
            {
                return result;
            }

            foreach(var fieldDeclaration in fieldsDeclarationList)
            {
#if DEBUG
                //GeneratorsHelper.ShowSyntaxNode(0, fieldDeclaration);
#endif

                var fieldDeclarationSyntax = (FieldDeclarationSyntax)fieldDeclaration;

                var item = new FieldItem()
                {
                    ClassDeclarationSyntaxNode = syntaxNode,
                    SyntaxNode = fieldDeclarationSyntax
                };

                var variableDeclaration = fieldDeclaration.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.VariableDeclaration));

                FillUpBaseFieldItem(variableDeclaration, item);

#if DEBUG
                //FileLogger.WriteLn($"item = {item}");
#endif

                result.Add(item);
            }

            return result;
        }

        private void FillUpBaseFieldItem(SyntaxNode syntaxNode, BaseFieldItem baseFieldItem)
        {
#if DEBUG
            //GeneratorsHelper.ShowSyntaxNode(0, syntaxNode);
#endif

            var predefinedType = syntaxNode.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.PredefinedType));

#if DEBUG
            //FileLogger.WriteLn($"predefinedType == null = {predefinedType == null}");
#endif

            if (predefinedType == null)
            {
                var identifierName = syntaxNode.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.IdentifierName));

#if DEBUG
                //FileLogger.WriteLn($"identifierName == null = {identifierName == null}");
#endif

                if (identifierName == null)
                {
                    var genericName = syntaxNode.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.GenericName));

#if DEBUG
                    //FileLogger.WriteLn($"genericName == null = {genericName == null}");
#endif

                    if(genericName == null)
                    {
#if DEBUG
                        FileLogger.WriteLn($"throw new NotImplementedException()");
#endif

                        throw new NotImplementedException();
                    }
                    else
                    {
#if DEBUG
                        //GeneratorsHelper.ShowSyntaxNode(0, genericName);
#endif

                        baseFieldItem.FieldTypeSyntaxNode = genericName;
                        baseFieldItem.KindFieldType = KindFieldType.GenericType;
                    }
                }
                else
                {
#if DEBUG
                    //GeneratorsHelper.ShowSyntaxNode(0, identifierName);
#endif

                    baseFieldItem.FieldTypeSyntaxNode = identifierName;
                    baseFieldItem.KindFieldType = KindFieldType.Identifier;
                }
            }
            else
            {
#if DEBUG
                //GeneratorsHelper.ShowSyntaxNode(0, predefinedType);
#endif

                baseFieldItem.FieldTypeSyntaxNode = predefinedType;

                var typeName = GeneratorsHelper.ToString(predefinedType.GetText());

#if DEBUG
                //FileLogger.WriteLn($"typeName = '{typeName}'");
                //FileLogger.WriteLn($"typeName == \"object\" = {typeName == "object"}");
#endif

                if(typeName == "object")
                {
                    baseFieldItem.KindFieldType = KindFieldType.Object;
                }
                else
                {
                    baseFieldItem.KindFieldType = KindFieldType.PredefinedType;
                }
            }
        }

        private void SaveFile(string source, string fileName)
        {
            var fileNameForSearch = $"\\{fileName}";

#if DEBUG
            FileLogger.WriteLn($"fileNameForSearch = {fileNameForSearch}");
#endif

            if (_pathsList.Any(p => p.EndsWith(fileNameForSearch)))
            {
                var path = _pathsList.First(p => p.EndsWith(fileNameForSearch));

#if DEBUG
                FileLogger.WriteLn($"path = {path}");
#endif

                File.WriteAllText(path, source, Encoding.UTF8);
            }
            else
            {
                _context.AddSource(fileName, SourceText.From(source, Encoding.UTF8));
            }
        }

#if DEBUG

#endif
    }
}
