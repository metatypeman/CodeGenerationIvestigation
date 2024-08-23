using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SymOntoClay.SourceGenerator;
using System.Linq;

namespace SourceGenerator
{
    public class PlainObjectsSearcher
    {
        public PlainObjectsSearcher(GeneratorExecutionContext context)
        {
            _context = context;
        }

        private readonly GeneratorExecutionContext _context;

        public void Run(TargetCompilationUnit targetCompilationUnit, PlainObjectsRegistry plainObjectsRegistry)
        {
            foreach (var targetClassItem in targetCompilationUnit.ClassItems)
            {
                ProcessTargetClassItem(targetClassItem, plainObjectsRegistry);
            }
        }

        private void ProcessTargetClassItem(TargetClassItem targetClassItem, PlainObjectsRegistry plainObjectsRegistry)
        {
#if DEBUG
            //GeneratorsHelper.ShowSyntaxNode(0, targetClassItem.SyntaxNode);
#endif

            var className = targetClassItem.SyntaxNode.Identifier.Text;

#if DEBUG
            //FileLogger.WriteLn($"className = '{className}'");
#endif

            var classFullName = $"{targetClassItem.Namespace}.{className}";

#if DEBUG
            //FileLogger.WriteLn($"classFullName = '{classFullName}'");
#endif

            var plainObjectClassName = GeneratorsHelper.GetPlainObjectClassIdentifier(targetClassItem.SyntaxNode);

#if DEBUG
            //FileLogger.WriteLn($"plainObjectClassName = '{plainObjectClassName}'");
#endif

            var plainObjectNamespace = GeneratorsHelper.GetPlainObjectNamespace(targetClassItem.Namespace);

#if DEBUG
            //FileLogger.WriteLn($"plainObjectNamespace = '{plainObjectNamespace}'");
#endif

            var plainObjectClassFullName = $"{plainObjectNamespace}.{plainObjectClassName}";

#if DEBUG
            //FileLogger.WriteLn($"plainObjectClassFullName = '{plainObjectClassFullName}'");
#endif

            var genericParamsCount = GetGenericParamsCount(targetClassItem);

#if DEBUG
            //FileLogger.WriteLn($"genericParamsCount = {genericParamsCount}");
#endif

            plainObjectsRegistry.Add(classFullName, genericParamsCount, plainObjectClassFullName);
        }

        private int GetGenericParamsCount(TargetClassItem targetClassItem)
        {
            var typeParameterList = targetClassItem.SyntaxNode?.ChildNodes().OfType<TypeParameterListSyntax>().FirstOrDefault();

#if DEBUG
            //GeneratorsHelper.ShowSyntaxNode(0, typeParameterList);
#endif

            if (typeParameterList == null)
            {
                return 0;
            }

#if DEBUG
            //FileLogger.WriteLn($"typeParameterList.Parameters.Count = {typeParameterList.Parameters.Count}");
#endif

            return typeParameterList.Parameters.Count;
        }
    }
}
