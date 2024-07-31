﻿using Microsoft.CodeAnalysis;
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
            //GeneratorsHelper.ShowSyntaxNode(0, targetClassItem.SyntaxNode);
#endif

            var identationStep = 4;
            var baseIdentation = 0;
            var classDeclIdentation = baseIdentation + identationStep;
            var classContentDeclIdentation = classDeclIdentation + identationStep;
            var classContentIdentation = classContentDeclIdentation + identationStep;

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
            sourceCodePlainObjectBuilder.AppendLine("using TestSandBox.Serialization;");
            sourceCodePlainObjectBuilder.AppendLine();
            sourceCodePlainObjectBuilder.AppendLine($"namespace {targetClassItem.Namespace}");
            sourceCodePlainObjectBuilder.AppendLine("{");
            sourceCodePlainObjectBuilder.AppendLine($"{GeneratorsHelper.Spaces(classDeclIdentation)}public partial class {plainObjectClassName}");
            sourceCodePlainObjectBuilder.AppendLine($"{GeneratorsHelper.Spaces(classDeclIdentation)}{{");
            foreach (var propertyItem in propertyItems) 
            {
#if DEBUG
                FileLogger.WriteLn($"propertyItem = {propertyItem}");
                GeneratorsHelper.ShowSyntaxNode(0, propertyItem.SyntaxNode);
#endif

                var identifier = GetPropertyIdentifier(propertyItem);

#if DEBUG
                FileLogger.WriteLn($"identifier = '{identifier}'");
#endif

                sourceCodePlainObjectBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}public {GetBaseFieldMemberType(propertyItem)} {GetPropertyIdentifier(propertyItem)} {{ get; set; }}");
            }
            foreach (var fieldItem in fieldsItems) 
            {
#if DEBUG
                FileLogger.WriteLn($"fieldItem = {fieldItem}");
                GeneratorsHelper.ShowSyntaxNode(0, fieldItem.SyntaxNode);
#endif

                var fieldIdentifier = GetFieldIdentifier(fieldItem);

#if DEBUG
                FileLogger.WriteLn($"fieldIdentifier = '{fieldIdentifier}'");
#endif

                sourceCodePlainObjectBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}public {GetBaseFieldMemberType(fieldItem)} {GetFieldIdentifier(fieldItem)};");
            }
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
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}Type ISerializable.GetPlainObjectType() => typeof({plainObjectClassName});");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}void ISerializable.OnWritePlainObject(object plainObject, ISerializer serializer)");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}OnWritePlainObject(({plainObjectClassName})plainObject, serializer);");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}}}");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}private void OnWritePlainObject({plainObjectClassName} plainObject, ISerializer serializer)");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}{{");
            foreach (var propertyItem in propertyItems)
            {
                sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}{CreateWriteProperty(propertyItem)}");
            }
            foreach (var fieldItem in fieldsItems)
            {
                sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}{CreateWriteField(fieldItem)}");
            }
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}}}");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}void ISerializable.OnReadPlainObject(object plainObject, IDeserializer deserializer)");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}OnReadPlainObject(({plainObjectClassName})plainObject, deserializer);");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}}}");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}private void OnReadPlainObject({plainObjectClassName} plainObject, IDeserializer deserializer)");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}{{");
            foreach (var propertyItem in propertyItems)
            {
                sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}{CreateReadProperty(propertyItem)}");
            }
            foreach (var fieldItem in fieldsItems)
            {
                sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}{CreateReadField(fieldItem)}");
            }
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}}}");
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

        private string GetBaseFieldMemberType(BaseFieldItem baseFieldItem)
        {
            if(baseFieldItem.KindFieldType == KindFieldType.PredefinedType)
            {
#if DEBUG
                FileLogger.WriteLn($"baseFieldItem = {baseFieldItem}");
                GeneratorsHelper.ShowSyntaxNode(0, baseFieldItem.FieldTypeSyntaxNode);
#endif

                return GeneratorsHelper.ToString(baseFieldItem.FieldTypeSyntaxNode.GetText());
            }
            else
            {
                return "ObjectPtr";
            }
        }

        private string GetClassIdentifier(ClassDeclarationSyntax syntaxNode)
        {
#if DEBUG
            //GeneratorsHelper.ShowSyntaxNode(0, syntaxNode);
#endif

            var sb = new StringBuilder(syntaxNode.Identifier.Text);

            var typeParameterList = syntaxNode?.ChildNodes().FirstOrDefault(p => p.IsKind(SyntaxKind.TypeParameterList));

#if DEBUG
            FileLogger.WriteLn("typeParameterList:");
            GeneratorsHelper.ShowSyntaxNode(0, typeParameterList);
#endif

            if(typeParameterList != null)
            {
                sb.Append(GeneratorsHelper.ToString(typeParameterList.GetText()));
            }

            return sb.ToString();
        }

        private string GetPlainObjectClassIdentifier(ClassDeclarationSyntax syntaxNode)
        {
            var sb = new StringBuilder(syntaxNode.Identifier.Text);
            sb.Append("Po");
            return sb.ToString();
        }

        private string GetPropertyIdentifier(PropertyItem propertyItem)
        {
            return GetPropertyIdentifier(propertyItem.SyntaxNode);
        }

        private string GetPropertyIdentifier(PropertyDeclarationSyntax syntaxNode)
        {
            return syntaxNode.Identifier.Text;
        }

        private string GetFieldIdentifier(FieldItem fieldItem)
        {
            return GetFieldIdentifier(fieldItem.SyntaxNode);
        }

        private string GetFieldIdentifier(FieldDeclarationSyntax syntaxNode)
        {
#if DEBUG
            GeneratorsHelper.ShowSyntaxNode(0, syntaxNode);
#endif

            var variableDeclarator = syntaxNode.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.VariableDeclaration))?.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.VariableDeclarator));

#if DEBUG
            FileLogger.WriteLn($"variableDeclarator == null = {variableDeclarator == null}");
            GeneratorsHelper.ShowSyntaxNode(0, variableDeclarator);
#endif

            return GeneratorsHelper.ToString(variableDeclarator.GetText());
        }

        private string GetTypeName(FieldItem fieldItem)
        {
            return GetTypeName(fieldItem.FieldTypeSyntaxNode);
        }

        private string GetTypeName(PropertyItem propertyItem)
        {
            return GetTypeName(propertyItem.FieldTypeSyntaxNode);
        }

        private string GetTypeName(SyntaxNode syntaxNode)
        {
#if DEBUG
            GeneratorsHelper.ShowSyntaxNode(0, syntaxNode);
#endif

            return GeneratorsHelper.ToString(syntaxNode.GetText()); ;
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

        private string CreateWriteProperty(PropertyItem propertyItem)
        {
            var propertyIdentifier = GetPropertyIdentifier(propertyItem);

            var sb = new StringBuilder("plainObject.");
            sb.Append(propertyIdentifier);
            sb.Append(" = ");
            switch(propertyItem.KindFieldType)
            {
                case KindFieldType.PredefinedType:
                    sb.Append(propertyIdentifier);
                    break;

                default:
                    sb.Append($"serializer.GetSerializedObjectPtr({propertyIdentifier})");
                    break;
            }
            sb.Append(";");

            return sb.ToString();
        }

        private string CreateWriteField(FieldItem fieldItem)
        {
            var fieldIndentifier = GetFieldIdentifier(fieldItem);

            var sb = new StringBuilder("plainObject.");
            sb.Append(fieldIndentifier);
            sb.Append(" = ");
            switch (fieldItem.KindFieldType)
            {
                case KindFieldType.PredefinedType:                    
                    sb.Append(fieldIndentifier);
                    break;

                default:
                    sb.Append($"serializer.GetSerializedObjectPtr({fieldIndentifier})");
                    break;
            }
            sb.Append(";");
            return sb.ToString();
        }

        private string CreateReadProperty(PropertyItem propertyItem)
        {
            var propertyIdentifier = GetPropertyIdentifier(propertyItem);

            var sb = new StringBuilder();
            sb.Append(propertyIdentifier);
            sb.Append(" = ");

            switch (propertyItem.KindFieldType)
            {
                case KindFieldType.PredefinedType:
                    sb.Append("plainObject.");
                    sb.Append(propertyIdentifier);
                    break;

                default:
                    {
                        var typeName = GetTypeName(propertyItem);

#if DEBUG
                        FileLogger.WriteLn($"typeName = '{typeName}'");
#endif

                        sb.Append($"deserializer.GetDeserializedObject<{typeName}>(plainObject.{propertyIdentifier})");
                    }
                    break;
            }
            sb.Append(";");
            return sb.ToString();
        }

        private string CreateReadField(FieldItem fieldItem)
        {
            var fieldIndentifier = GetFieldIdentifier(fieldItem);

            var sb = new StringBuilder();
            sb.Append(fieldIndentifier);
            sb.Append(" = ");

            switch (fieldItem.KindFieldType)
            {
                case KindFieldType.PredefinedType:
                    sb.Append("plainObject.");
                    sb.Append(fieldIndentifier);
                    break;

                default:
                    {
                        var typeName = GetTypeName(fieldItem);

#if DEBUG
                        FileLogger.WriteLn($"typeName = '{typeName}'");
#endif

                        sb.Append($"deserializer.GetDeserializedObject<{typeName}>(plainObject.{fieldIndentifier})");
                    }
                    break;
            }
            sb.Append(";");
            return sb.ToString();
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
