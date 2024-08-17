using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace SourceGenerator
{
    public class CodeChunksSearcher
    {
        public CodeChunksSearcher(GeneratorExecutionContext context)
        {
            _context = context;
        }
        
        private readonly GeneratorExecutionContext _context;

        private List<string> _targetConstructors = new List<string>()
        {
            "LoggedCodeChunkFunctorWithoutResult",
            "OtherLoggedCodeChunkFunctorWithoutResult"
        };

        private List<(string FirstIdenfifier, string SecondIdenfifier)> _targetInvocations = new List<(string FirstIdenfifier, string SecondIdenfifier)>()
        {
            ("OtherLoggedCodeChunkFunctorWithoutResult", "Run"),
            ("LoggedCodeChunkFunctorWithoutResult", "Run"),
            (null, "CreateCodeChunk")
        };

        public List<TargetCodeChunksCompilationUnit> Run()
        {
            var result = new List<TargetCodeChunksCompilationUnit>();

            var syntaxTrees = _context.Compilation.SyntaxTrees;

            var context = new CodeChunkSearchingContext();

            foreach (var syntaxTree in syntaxTrees)
            {
                if (syntaxTree.FilePath.EndsWith(".g.cs"))
                {
                    continue;
                }

                var codeChunkItemsResult = new List<CodeChunkItem>();
                var usings = new List<string>();

                ProcessSyntaxTree(syntaxTree, context, ref codeChunkItemsResult, ref usings);

#if DEBUG
                //FileLogger.WriteLn($"codeChunkItemsResult.Count = {codeChunkItemsResult.Count}");
#endif

                if (codeChunkItemsResult.Count > 0)
                {
#if DEBUG
                    //FileLogger.WriteLn($"usings.Count = {usings.Count}");
                    //foreach (var usingItem in usings)
                    //{
                    //    FileLogger.WriteLn($"usingItem = '{usingItem}'");
                    //}
#endif

                    var item = new TargetCodeChunksCompilationUnit()
                    {
                        FilePath = syntaxTree.FilePath,
                        CodeChunkItems = codeChunkItemsResult,
                        Usings = usings
                    };

                    result.Add(item);
                }
            }

            return result;
        }

        private void ProcessSyntaxTree(SyntaxTree syntaxTree, CodeChunkSearchingContext context, ref List<CodeChunkItem> result, ref List<string> usings)
        {
#if DEBUG
            //FileLogger.WriteLn($"syntaxTree.FilePath = {syntaxTree.FilePath}");
#endif

            var root = syntaxTree.GetRoot();

#if DEBUG
            //GeneratorsHelper.ShowSyntaxNode(0, root);
            //FileLogger.WriteLn($"root?.GetKind() = {root?.Kind()}");
            //FileLogger.WriteLn($"root?.GetText() = {root?.GetText()}");
#endif

            var childNodes = root?.ChildNodes();

            if (childNodes == null)
            {
                return;
            }

            var namespaceDeclarations = childNodes.Where(p => p.IsKind(SyntaxKind.NamespaceDeclaration));

#if DEBUG
            //FileLogger.WriteLn($"namespaceDeclarations.Count() = {namespaceDeclarations.Count()}");
#endif

            if (namespaceDeclarations.Count() == 0)
            {
                return;
            }

            var localUsings = childNodes.Where(p => p.IsKind(SyntaxKind.UsingDirective)).Select(p => GeneratorsHelper.ToString(p.GetText()));

#if DEBUG
            //FileLogger.WriteLn($"localUsings.Count() = {localUsings.Count()}");
            //foreach (var localUsing in localUsings)
            //{
            //    FileLogger.WriteLn($"localUsing = '{localUsing}'");
            //}
#endif

            if (localUsings.Any())
            {
                usings.AddRange(localUsings);
            }

            context.FilePath = syntaxTree.FilePath;

#if DEBUG
            //FileLogger.WriteLn($"context = {context}");
#endif

            foreach (var namespaceDeclaration in namespaceDeclarations)
            {
                ProcessNamespaceDeclaration(namespaceDeclaration, context, ref result);
            }
        }

        private void ProcessNamespaceDeclaration(SyntaxNode namespaceDeclaration, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
#if DEBUG
            //FileLogger.WriteLn($"namespaceDeclaration?.GetKind() = {namespaceDeclaration?.Kind()}");
            //FileLogger.WriteLn($"namespaceDeclaration?.GetText() = {namespaceDeclaration?.GetText()}");
#endif

            var childNodes = namespaceDeclaration?.ChildNodes();

            var classDeclarations = childNodes.Where(p => p.IsKind(SyntaxKind.ClassDeclaration));

#if DEBUG
            //FileLogger.WriteLn($"classDeclarations.Count() = {classDeclarations.Count()}");
#endif

            if (classDeclarations.Count() == 0)
            {
                return;
            }

            var namespaceIdentifierNode = childNodes.Single(p => p.IsKind(SyntaxKind.QualifiedName) || p.IsKind(SyntaxKind.IdentifierName));

#if DEBUG
            //FileLogger.WriteLn($"namespaceIdentifierNode?.GetKind() = {namespaceIdentifierNode?.Kind()}");
            //FileLogger.WriteLn($"namespaceIdentifierNode?.GetText() = {namespaceIdentifierNode?.GetText()}");
#endif

            var namespaceIdentifier = GeneratorsHelper.ToString(namespaceIdentifierNode?.GetText());

#if DEBUG
            //FileLogger.WriteLn($"namespaceIdentifier = '{namespaceIdentifier}'");
#endif

            context.Namespace = namespaceIdentifier;

#if DEBUG
            //FileLogger.WriteLn($"context = {context}");
#endif

            foreach (var classDeclaration in classDeclarations)
            {
                ProcessClassDeclaration(classDeclaration, context, ref result);
            }
        }

        private void ProcessClassDeclaration(SyntaxNode classDeclaration, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
#if DEBUG
            //FileLogger.WriteLn($"classDeclaration?.GetKind() = {classDeclaration?.Kind()}");
            //FileLogger.WriteLn($"classDeclaration?.GetText() = {classDeclaration?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, classDeclaration);
#endif

            var childNodes = classDeclaration?.ChildNodes();

#if DEBUG
            //FileLogger.WriteLn($"childNodes.Count() = {childNodes.Count()}");
#endif

            var methodDeclarations = childNodes.Where(p => p.IsKind(SyntaxKind.MethodDeclaration));

            if (methodDeclarations.Count() == 0)
            {
                return;
            }

            foreach (var methodDeclaration in methodDeclarations)
            {
                ProcessMethodDeclaration(methodDeclaration, context, ref result);
            }
        }

        private void ProcessMethodDeclaration(SyntaxNode methodDeclaration, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
#if DEBUG
            //FileLogger.WriteLn($"methodDeclaration?.GetKind() = {methodDeclaration?.Kind()}");
            //FileLogger.WriteLn($"methodDeclaration?.GetText() = {methodDeclaration?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, methodDeclaration);
#endif

            var childNodes = methodDeclaration?.ChildNodes();

#if DEBUG
            //FileLogger.WriteLn($"childNodes.Count() = {childNodes.Count()}");
#endif

            var block = childNodes.FirstOrDefault(p => p.IsKind(SyntaxKind.Block));

            ProcessMethodBlock(block, context, ref result);
        }

        private void ProcessMethodBlock(SyntaxNode block, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
#if DEBUG
            //FileLogger.WriteLn($"block?.GetKind() = {block?.Kind()}");
            //FileLogger.WriteLn($"block?.GetText() = {block?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, block);
#endif

            var childNodes = block?.ChildNodes();

#if DEBUG
            //FileLogger.WriteLn($"childNodes.Count() = {childNodes.Count()}");
#endif

            if (childNodes.Count() == 0)
            {
                return;
            }

            foreach (var childNode in childNodes)
            {
                ProcessMethodBlockChildNodes(childNode, context, ref result);
            }
        }

        private void ProcessMethodBlockChildNodes(SyntaxNode node, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
#if DEBUG
            //FileLogger.WriteLn($"node?.GetKind() = {node?.Kind()}");
            //FileLogger.WriteLn($"node?.GetText() = {node?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, node);
#endif

            if (node.IsKind(SyntaxKind.InvocationExpression))
            {
                if (ShouldCatchInvocationExpression(node))
                {
#if DEBUG
                    //FileLogger.WriteLn($"InvocationExpression !!!!!!!!!");
                    //FileLogger.WriteLn($"node?.GetKind() = {node?.Kind()}");
                    //FileLogger.WriteLn($"node?.GetText() = {node?.GetText()}");
                    //GeneratorsHelper.ShowSyntaxNode(0, node);
#endif

                    ProcessTargetInvocationExpression(node, context, ref result);
                }
            }
            else
            {
                if (node.IsKind(SyntaxKind.ObjectCreationExpression) && ShouldCatchObjectCreationExpression(node))
                {
#if DEBUG
                    //FileLogger.WriteLn($"ObjectCreationExpression !!!!!!!!!");
                    //FileLogger.WriteLn($"node?.GetKind() = {node?.Kind()}");
                    //FileLogger.WriteLn($"node?.GetText() = {node?.GetText()}");
                    //GeneratorsHelper.ShowSyntaxNode(0, node);
#endif

                    ProcessTargetObjectCreationExpression(node, context, ref result);
                }
            }

            var childNodes = node?.ChildNodes();

#if DEBUG
            //FileLogger.WriteLn($"childNodes.Count() = {childNodes.Count()}");
#endif

            foreach (var childNode in childNodes)
            {
                ProcessMethodBlockChildNodes(childNode, context, ref result);
            }
        }

        private void ProcessTargetInvocationExpression(SyntaxNode node, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
#if DEBUG
            //FileLogger.WriteLn($"node?.GetKind() = {node?.Kind()}");
            //FileLogger.WriteLn($"node?.GetText() = {node?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, node);
#endif

            ProcessTargetExpression(node, context, ref result);
        }

        private void ProcessTargetObjectCreationExpression(SyntaxNode node, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
#if DEBUG
            //FileLogger.WriteLn($"node?.GetKind() = {node?.Kind()}");
            //FileLogger.WriteLn($"node?.GetText() = {node?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, node);
#endif

            ProcessTargetExpression(node, context, ref result);
        }

        private void ProcessTargetExpression(SyntaxNode node, CodeChunkSearchingContext context, ref List<CodeChunkItem> result)
        {
#if DEBUG
            //FileLogger.WriteLn($"node?.GetKind() = {node?.Kind()}");
            //FileLogger.WriteLn($"node?.GetText() = {node?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, node);
#endif

            var argumentList = node.ChildNodes().FirstOrDefault(p => p.IsKind(SyntaxKind.ArgumentList));

            if (argumentList == null)
            {
                return;
            }

#if DEBUG
            //FileLogger.WriteLn($"argumentList?.GetKind() = {argumentList?.Kind()}");
            //FileLogger.WriteLn($"argumentList?.GetText() = {argumentList?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, argumentList);
#endif

            var identifierArgument = argumentList.ChildNodes().FirstOrDefault(p => p.ChildNodes().Any(x => x.IsKind(SyntaxKind.StringLiteralExpression)));

            if (identifierArgument == null)
            {
                return;
            }

#if DEBUG
            //FileLogger.WriteLn($"identifierArgument?.GetKind() = {identifierArgument?.Kind()}");
            //FileLogger.WriteLn($"identifierArgument?.GetText() = {identifierArgument?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, identifierArgument);
#endif

            var identifier = GeneratorsHelper.ToString(identifierArgument.GetText()).Replace('"', ' ').Trim();

#if DEBUG
            //FileLogger.WriteLn($"identifier = '{identifier}'");
#endif

            var lambdas = argumentList.ChildNodes().Where(p => p.ChildNodes().Any(x => x.IsKind(SyntaxKind.ParenthesizedLambdaExpression)))
                .SelectMany(p => p.ChildNodes().Where(x => x.IsKind(SyntaxKind.ParenthesizedLambdaExpression))).Select(p => p as ParenthesizedLambdaExpressionSyntax).ToList();

            if (lambdas.Count() == 0)
            {
                return;
            }


//            foreach (var lambda in lambdas)
//            {
//#if DEBUG
//                FileLogger.WriteLn($"lambda?.GetKind() = {lambda?.Kind()}");
//                FileLogger.WriteLn($"lambda?.GetText() = {lambda?.GetText()}");
//                //GeneratorsHelper.ShowSyntaxNode(0, lambda);
//#endif


//            }

            var resultItem = new CodeChunkItem()
            {
                Identifier = identifier,
                Namespace = context.Namespace,
                Lambdas = lambdas
            };

#if DEBUG
            //FileLogger.WriteLn($"resultItem = {resultItem}");
#endif

            result.Add(resultItem);
        }

        private bool ShouldCatchInvocationExpression(SyntaxNode node)
        {
#if DEBUG
            //FileLogger.WriteLn($"node?.GetKind() = {node?.Kind()}");
            //FileLogger.WriteLn($"node?.GetText() = {node?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, node);
#endif

            var simpleMemberAccessExpression = node.ChildNodes().FirstOrDefault(p => p.IsKind(SyntaxKind.SimpleMemberAccessExpression));

#if DEBUG
            //FileLogger.WriteLn($"simpleMemberAccessExpression == null = {simpleMemberAccessExpression == null}");
#endif

            if (simpleMemberAccessExpression == null)
            {
                return false;
            }

#if DEBUG
            //FileLogger.WriteLn($"simpleMemberAccessExpression?.GetKind() = {simpleMemberAccessExpression?.Kind()}");
            //FileLogger.WriteLn($"simpleMemberAccessExpression?.GetText() = {simpleMemberAccessExpression?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, simpleMemberAccessExpression);
#endif

            var childNodes = simpleMemberAccessExpression?.ChildNodes();

#if DEBUG
            //FileLogger.WriteLn($"childNodes.Count() = {childNodes.Count()}");
#endif

            if (childNodes.Count() != 2)
            {
                return false;
            }

            var firstIdentifier = string.Empty;
            var secondIdentifier = string.Empty;

            var firstSyntaxNode = childNodes.ElementAt(0);

#if DEBUG
            //FileLogger.WriteLn($"firstSyntaxNode?.GetKind() = {firstSyntaxNode?.Kind()}");
            //FileLogger.WriteLn($"firstSyntaxNode?.GetText() = {firstSyntaxNode?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, firstSyntaxNode);
#endif

            if (firstSyntaxNode.IsKind(SyntaxKind.IdentifierName))
            {
                var identifierNameSyntax = firstSyntaxNode as IdentifierNameSyntax;

                if (identifierNameSyntax == null)
                {
                    return false;
                }

                firstIdentifier = identifierNameSyntax.Identifier.Text;
            }
            else
            {
                if (firstSyntaxNode.IsKind(SyntaxKind.GenericName))
                {
                    var genericNameSyntax = firstSyntaxNode as GenericNameSyntax;

                    if (genericNameSyntax == null)
                    {
                        return false;
                    }

                    firstIdentifier = genericNameSyntax.Identifier.Text;
                }
                else
                {
                    return false;
                }
            }

            var secondSyntaxNode = childNodes.ElementAt(1);

#if DEBUG
            //FileLogger.WriteLn($"secondSyntaxNode?.GetKind() = {secondSyntaxNode?.Kind()}");
            //FileLogger.WriteLn($"secondSyntaxNode?.GetText() = {secondSyntaxNode?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, secondSyntaxNode);
#endif

            if (secondSyntaxNode.IsKind(SyntaxKind.IdentifierName))
            {
                var identifierNameSyntax = secondSyntaxNode as IdentifierNameSyntax;

                if (identifierNameSyntax == null)
                {
                    return false;
                }

                secondIdentifier = identifierNameSyntax.Identifier.Text;
            }
            else
            {
                if (secondSyntaxNode.IsKind(SyntaxKind.GenericName))
                {
                    var genericNameSyntax = secondSyntaxNode as GenericNameSyntax;

                    if (genericNameSyntax == null)
                    {
                        return false;
                    }

                    secondIdentifier = genericNameSyntax.Identifier.Text;
                }
                else
                {
                    return false;
                }
            }

#if DEBUG
            //FileLogger.WriteLn($"firstIdentifier = '{firstIdentifier}'");
            //FileLogger.WriteLn($"secondIdentifier = '{secondIdentifier}'");
#endif

            return _targetInvocations.Any(p => (string.IsNullOrWhiteSpace(p.FirstIdenfifier) || p.FirstIdenfifier == firstIdentifier) &&
                (string.IsNullOrWhiteSpace(p.SecondIdenfifier) || p.SecondIdenfifier == secondIdentifier));
        }

        private bool ShouldCatchObjectCreationExpression(SyntaxNode node)
        {
#if DEBUG
            //FileLogger.WriteLn($"node?.GetKind() = {node?.Kind()}");
            //FileLogger.WriteLn($"node?.GetText() = {node?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, node);
#endif

            var genericName = node.ChildNodes().FirstOrDefault(p => p.IsKind(SyntaxKind.GenericName));

            if (genericName == null)
            {
                var identifierName = node.ChildNodes().FirstOrDefault(p => p.IsKind(SyntaxKind.IdentifierName));

                if (identifierName == null)
                {
                    return false;
                }

                var identifierNameSyntax = identifierName as IdentifierNameSyntax;

                if (identifierNameSyntax != null)
                {
                    return false;
                }

                return _targetConstructors.Contains(identifierNameSyntax.Identifier.Text);
            }

#if DEBUG
            //FileLogger.WriteLn($"node?.GetKind() = {node?.Kind()}");
            //FileLogger.WriteLn($"node?.GetText() = {node?.GetText()}");
            //GeneratorsHelper.ShowSyntaxNode(0, node);
#endif

            var genericNameSyntax = genericName as GenericNameSyntax;

            if (genericNameSyntax == null)
            {
                return false;
            }

#if DEBUG
            //FileLogger.WriteLn($"node?.GetKind() = {node?.Kind()}");
            //FileLogger.WriteLn($"node?.GetText() = {node?.GetText()}");
            //FileLogger.WriteLn($"genericNameSyntax.Identifier.Text = '{genericNameSyntax.Identifier.Text}'");
            //GeneratorsHelper.ShowSyntaxNode(0, node);
#endif

            return _targetConstructors.Contains(genericNameSyntax.Identifier.Text);
        }
    }
}
