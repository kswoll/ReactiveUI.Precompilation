using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace ReactiveUI.Precompilation.Modules
{
    public class ClassesCollector : SymbolWalker
    {
        public static IReadOnlyList<INamedTypeSymbol> CollectClasses(Compilation compilation)
        {
            var collector = new ClassesCollector();
            compilation.Assembly.Accept(collector);
            return collector.classes;
        }

        private readonly List<INamedTypeSymbol> classes = new List<INamedTypeSymbol>();

        private ClassesCollector()
        {
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            switch (symbol.TypeKind)
            {
                case TypeKind.Class:
                case TypeKind.Struct:
                    classes.Add(symbol);
                    break;
            }
            base.VisitNamedType(symbol);
        }
    }
}