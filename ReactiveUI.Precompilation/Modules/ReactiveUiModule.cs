using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StackExchange.Precompilation;

namespace ReactiveUI.Precompilation.Modules
{
    public class ReactiveUIModule : ICompileModule
    {
        public void BeforeCompile(BeforeCompileContext context)
        {
//            Debugger.Launch();
//            context.Diagnostics.Add(new Diagnostic());

            try
            {
                var reactiveAttribute = context.Compilation.GetTypeByMetadataName("ReactiveUI.Precompilation.ReactiveAttribute");
                var observableAsPropertyAttribute = context.Compilation.GetTypeByMetadataName("ReactiveUI.Precompilation.ObservableAsPropertyAttribute");
                var iReactiveObject = context.Compilation.GetTypeByMetadataName("ReactiveUI.IReactiveObject");
                var reactiveClasses = ClassesCollector.CollectClasses(context.Compilation)
                    .Where(x => iReactiveObject.IsAssignableFrom(x) && x.GetMembers().Any(y => y.HasAttribute(reactiveAttribute, observableAsPropertyAttribute)));
                var reactiveSyntaxTrees = reactiveClasses
                    .SelectMany(x => x.DeclaringSyntaxReferences)
                    .Select(x => x.SyntaxTree)
                    .Distinct()
                    .ToArray();

                var reactiveRewriter = new ReactiveRewriter(context, reactiveAttribute, observableAsPropertyAttribute);
//            var replacements = new List<Action>();
                foreach (var syntaxTree in reactiveSyntaxTrees)
                {
                    var compilationUnit = (CompilationUnitSyntax)syntaxTree.GetRoot();
                    var newCompilationUnit = compilationUnit.Accept(reactiveRewriter);
                    var newSyntaxTree = syntaxTree.WithRootAndOptions(newCompilationUnit, syntaxTree.Options);
                    context.Compilation = context.Compilation.ReplaceSyntaxTree(syntaxTree, newSyntaxTree);
                }
            }
            catch (Exception ex)
            {
                context.AddDiagnostic("Rx9999", ex.Message, ex.ToString());
            }
        }

        public void AfterCompile(AfterCompileContext context)
        {
        }
    }
}