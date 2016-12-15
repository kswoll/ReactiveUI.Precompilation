using Microsoft.CodeAnalysis;

namespace ReactiveUI.Precompilation.Modules
{
    public class SymbolWalker : SymbolVisitor
    {
        public override void VisitAlias(IAliasSymbol symbol)
        {
            base.VisitAlias(symbol);
        }

        public override void VisitArrayType(IArrayTypeSymbol symbol)
        {
            base.VisitArrayType(symbol);
        }

        public override void VisitAssembly(IAssemblySymbol symbol)
        {
            base.VisitAssembly(symbol);
            symbol.GlobalNamespace.Accept(this);
        }

        public override void VisitDynamicType(IDynamicTypeSymbol symbol)
        {
            base.VisitDynamicType(symbol);
        }

        public override void VisitEvent(IEventSymbol symbol)
        {
            base.VisitEvent(symbol);
        }

        public override void VisitField(IFieldSymbol symbol)
        {
            base.VisitField(symbol);
        }

        public override void VisitLabel(ILabelSymbol symbol)
        {
            base.VisitLabel(symbol);
        }

        public override void VisitLocal(ILocalSymbol symbol)
        {
            base.VisitLocal(symbol);
        }

        public override void VisitMethod(IMethodSymbol symbol)
        {
            base.VisitMethod(symbol);
        }

        public override void VisitModule(IModuleSymbol symbol)
        {
            base.VisitModule(symbol);
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            base.VisitNamedType(symbol);
            foreach (var item in symbol.GetMembers())
            {
                item.Accept(this);
            }
            foreach (var item in symbol.GetTypeMembers())
            {
                item.Accept(this);
            }
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            base.VisitNamespace(symbol);
            foreach (var item in symbol.GetTypeMembers())
            {
                item.Accept(this);
            }
            foreach (var item in symbol.GetNamespaceMembers())
            {
                item.Accept(this);
            }
        }

        public override void VisitParameter(IParameterSymbol symbol)
        {
            base.VisitParameter(symbol);
        }

        public override void VisitPointerType(IPointerTypeSymbol symbol)
        {
            base.VisitPointerType(symbol);
        }

        public override void VisitProperty(IPropertySymbol symbol)
        {
            base.VisitProperty(symbol);
        }

        public override void VisitRangeVariable(IRangeVariableSymbol symbol)
        {
            base.VisitRangeVariable(symbol);
        }

        public override void VisitTypeParameter(ITypeParameterSymbol symbol)
        {
            base.VisitTypeParameter(symbol);
        }
    }
}