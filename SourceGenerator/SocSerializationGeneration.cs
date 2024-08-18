﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace SymOntoClay.SourceGenerator
{
    public class SocSerializationGeneration
    {
        public SocSerializationGeneration(GeneratorExecutionContext context)
        {
            _context = context;
        }

        private readonly GeneratorExecutionContext _context;
        
        public void Run(TargetCompilationUnit targetCompilationUnit)
        {
            var requredNamespaces = new List<string>()
            {
                "using System.Text;",
                "using System;",
                "using SymOntoClay.Common;",
                "using SymOntoClay.Common.DebugHelpers;",
                "using SymOntoClay.Serialization;"
            };

            if(targetCompilationUnit.Usings?.Any() ?? false)
            {
                requredNamespaces.AddRange(targetCompilationUnit.Usings);
            }

            var sourceCodeBuilder = new StringBuilder();
            sourceCodeBuilder.AppendLine("// <autogenerated />");
            foreach(var item in requredNamespaces.Distinct())
            {
                sourceCodeBuilder.AppendLine(item);
            }
            
            foreach (var targetClassItem in targetCompilationUnit.ClassItems)
            {
                ProcessTargetClassItem(targetClassItem, sourceCodeBuilder);
            }

            var firstClassItem = targetCompilationUnit.ClassItems.First();

            var fileName = $"{firstClassItem.Namespace}.{firstClassItem.Identifier}.g.cs";

            SaveFile(sourceCodeBuilder.ToString(), fileName);
        }

        private void ProcessTargetClassItem(TargetClassItem targetClassItem, StringBuilder sourceCodeBuilder)
        {
            var identationStep = 4;
            var baseIdentation = 0;
            var classDeclIdentation = baseIdentation + identationStep;
            var classContentDeclIdentation = classDeclIdentation + identationStep;
            var classContentIdentation = classContentDeclIdentation + identationStep;

            var plainObjectClassName = GetPlainObjectClassIdentifier(targetClassItem.SyntaxNode);

            var propertyItems = GetPropertyItems(targetClassItem.SyntaxNode);

            var fieldsItems = GetFieldItems(targetClassItem.SyntaxNode);

            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"namespace {targetClassItem.Namespace}.PlainObjects");
            sourceCodeBuilder.AppendLine("{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classDeclIdentation)}public partial class {plainObjectClassName}: IObjectToString");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classDeclIdentation)}{{");
            foreach (var propertyItem in propertyItems)
            {
                sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}public {GetBaseFieldMemberType(propertyItem)} {propertyItem.Identifier} {{ get; set; }}");
            }
            foreach (var fieldItem in fieldsItems)
            {
                sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}public {GetBaseFieldMemberType(fieldItem)} {fieldItem.Identifier};");
            }
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}/// <inheritdoc/>");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}public override string ToString()");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}return ToString(0u);");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}}}");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}/// <inheritdoc/>");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}public string ToString(uint n)");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}return this.GetDefaultToStringInformation(n);");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}}}");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}/// <inheritdoc/>");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}string IObjectToString.PropertiesToString(uint n)");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}var spaces = DisplayHelper.Spaces(n);");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}var sb = new StringBuilder();");
            foreach (var propertyItem in propertyItems)
            {
                sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}sb.AppendLine($\"{{spaces}}{{nameof({propertyItem.Identifier})}} = {{{propertyItem.Identifier}}}\");");
            }
            foreach (var fieldItem in fieldsItems)
            {
                sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}sb.AppendLine($\"{{spaces}}{{nameof({fieldItem.Identifier})}} = {{{fieldItem.Identifier}}}\");");
            }
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}return sb.ToString();");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}}}");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classDeclIdentation)}}}");
            sourceCodeBuilder.AppendLine("}");

            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"namespace {targetClassItem.Namespace}");
            sourceCodeBuilder.AppendLine("{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classDeclIdentation)}public partial class {GetClassIdentifier(targetClassItem.SyntaxNode)}: ISerializable");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classDeclIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}Type ISerializable.GetPlainObjectType() => typeof(PlainObjects.{plainObjectClassName});");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}void ISerializable.OnWritePlainObject(object plainObject, ISerializer serializer)");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}{{");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}OnWritePlainObject((PlainObjects.{plainObjectClassName})plainObject, serializer);");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}}}");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}private void OnWritePlainObject(PlainObjects.{plainObjectClassName} plainObject, ISerializer serializer)");
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
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentIdentation)}OnReadPlainObject((PlainObjects.{plainObjectClassName})plainObject, deserializer);");
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}}}");
            sourceCodeBuilder.AppendLine();
            sourceCodeBuilder.AppendLine($"{GeneratorsHelper.Spaces(classContentDeclIdentation)}private void OnReadPlainObject(PlainObjects.{plainObjectClassName} plainObject, IDeserializer deserializer)");
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
        }

        private string GetBaseFieldMemberType(BaseFieldItem baseFieldItem)
        {
            if (baseFieldItem.KindFieldType == KindFieldType.PredefinedType)
            {
                return GeneratorsHelper.ToString(baseFieldItem.FieldTypeSyntaxNode.GetText());
            }
            else
            {
                return "ObjectPtr";
            }
        }

        private string GetClassIdentifier(ClassDeclarationSyntax syntaxNode)
        {
            var sb = new StringBuilder(syntaxNode.Identifier.Text);

            var typeParameterList = syntaxNode?.ChildNodes().FirstOrDefault(p => p.IsKind(SyntaxKind.TypeParameterList));

            if (typeParameterList != null)
            {
                sb.Append(GeneratorsHelper.ToString(typeParameterList.GetText()));
            }

            return sb.ToString();
        }

        private string GetPlainObjectClassIdentifier(ClassDeclarationSyntax syntaxNode)
        {
            var sb = new StringBuilder(syntaxNode.Identifier.Text);
            sb.Append("Po");

            var typeParameterList = syntaxNode?.ChildNodes().FirstOrDefault(p => p.IsKind(SyntaxKind.TypeParameterList));

            if (typeParameterList != null)
            {
                sb.Append(GeneratorsHelper.ToString(typeParameterList.GetText()).Replace("<", "_").Replace(",", "_").Replace(" ", string.Empty).Replace(">", string.Empty));
            }

            return sb.ToString();
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
            return GeneratorsHelper.ToString(syntaxNode.GetText()); ;
        }

        private List<PropertyItem> GetPropertyItems(ClassDeclarationSyntax syntaxNode)
        {
            var result = new List<PropertyItem>();

            var propertiesDeclarationsList = syntaxNode.ChildNodes()?.Where(p => p.IsKind(SyntaxKind.PropertyDeclaration) && IsSerializedProperty(p)) ?? new List<SyntaxNode>();

            if (propertiesDeclarationsList.Count() == 0)
            {
                return result;
            }

            foreach (var propertyDeclaration in propertiesDeclarationsList)
            {
                var propertyDeclarationSyntax = (PropertyDeclarationSyntax)propertyDeclaration;

                var item = new PropertyItem()
                {
                    ClassDeclarationSyntaxNode = syntaxNode,
                    SyntaxNode = propertyDeclarationSyntax
                };

                FillUpBaseFieldItem(propertyDeclaration, item);

                result.Add(item);
            }

            return result;
        }

        private static bool IsSerializedProperty(SyntaxNode propertyDeclarationNode)
        {
            var accessorList = propertyDeclarationNode.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.AccessorList));

            if (accessorList == null)
            {
                return false;
            }

            var getAccessorDeclaration = accessorList.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.GetAccessorDeclaration));

            if ((getAccessorDeclaration?.ChildNodes()?.Count() ?? 0) > 0)
            {
                return false;
            }

            var setAccessorDeclaration = accessorList.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.SetAccessorDeclaration));

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

            if (fieldsDeclarationList.Count() == 0)
            {
                return result;
            }

            foreach (var fieldDeclaration in fieldsDeclarationList)
            {
                var fieldDeclarationSyntax = (FieldDeclarationSyntax)fieldDeclaration;

                var item = new FieldItem()
                {
                    ClassDeclarationSyntaxNode = syntaxNode,
                    SyntaxNode = fieldDeclarationSyntax
                };

                var variableDeclaration = fieldDeclaration.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.VariableDeclaration));

                FillUpBaseFieldItem(variableDeclaration, item);

                result.Add(item);
            }

            return result;
        }

        private void FillUpBaseFieldItem(SyntaxNode syntaxNode, BaseFieldItem baseFieldItem)
        {
            var predefinedType = syntaxNode.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.PredefinedType));

            if (predefinedType == null)
            {
                var identifierName = syntaxNode.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.IdentifierName));

                if (identifierName == null)
                {
                    var genericName = syntaxNode.ChildNodes()?.FirstOrDefault(p => p.IsKind(SyntaxKind.GenericName));

                    if (genericName == null)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        baseFieldItem.FieldTypeSyntaxNode = genericName;
                        baseFieldItem.KindFieldType = KindFieldType.GenericType;
                    }
                }
                else
                {
                    baseFieldItem.FieldTypeSyntaxNode = identifierName;
                    baseFieldItem.KindFieldType = KindFieldType.Identifier;
                }
            }
            else
            {
                baseFieldItem.FieldTypeSyntaxNode = predefinedType;

                var typeName = GeneratorsHelper.ToString(predefinedType.GetText());

                if (typeName == "object")
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
            var propertyIdentifier = propertyItem.Identifier;

            var sb = new StringBuilder("plainObject.");
            sb.Append(propertyIdentifier);
            sb.Append(" = ");
            switch (propertyItem.KindFieldType)
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
            var fieldIndentifier = fieldItem.Identifier;

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
            var propertyIdentifier = propertyItem.Identifier;

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
                        sb.Append($"deserializer.GetDeserializedObject<{typeName}>(plainObject.{propertyIdentifier})");
                    }
                    break;
            }
            sb.Append(";");
            return sb.ToString();
        }

        private string CreateReadField(FieldItem fieldItem)
        {
            var fieldIndentifier = fieldItem.Identifier;

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
                        sb.Append($"deserializer.GetDeserializedObject<{typeName}>(plainObject.{fieldIndentifier})");
                    }
                    break;
            }
            sb.Append(";");
            return sb.ToString();
        }

        private void SaveFile(string source, string fileName)
        {
            _context.AddSource(fileName, SourceText.From(source, Encoding.UTF8));
        }
    }
}
