using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StackExchange.Precompilation;

namespace ReactiveUI.Precompilation.Modules
{
    public class ReactiveRewriter : CSharpSyntaxRewriter
    {
        private readonly BeforeCompileContext context;
        private readonly List<PropertyRewriter> propertyRewriters = new List<PropertyRewriter>();

        public ReactiveRewriter(BeforeCompileContext context, INamedTypeSymbol reactiveAttribute, INamedTypeSymbol observableAsPropertyAttribute)
        {
            this.context = context;
            propertyRewriters.Add(new ReactivePropertyRewriter(context, reactiveAttribute));
            propertyRewriters.Add(new ObservableAsPropertyRewriter(context, observableAsPropertyAttribute));
        }

        public override SyntaxList<TNode> VisitList<TNode>(SyntaxList<TNode> list)
        {
            if (typeof(MemberDeclarationSyntax) == typeof(TNode))
            {
                var members = list.SelectMany(x => RewriteProperty((MemberDeclarationSyntax)(object)x)).Cast<TNode>().ToList();
                var result = SyntaxFactory.List(members);
                return result;
            }
            else
            {
                return base.VisitList(list);
            }
        }

        public IEnumerable<MemberDeclarationSyntax> RewriteProperty(MemberDeclarationSyntax member)
        {
            var property = member as PropertyDeclarationSyntax;
            if (property != null)
            {
                var propertySymbol = context.Compilation.GetSemanticModel(property.SyntaxTree).GetDeclaredSymbol(property);
                foreach (var propertyRewriter in propertyRewriters)
                {
                    if (propertyRewriter.IsApplicable(propertySymbol))
                    {
                        return propertyRewriter.RewriteProperty(property);
                    }
                }
            }
            return new[] { (MemberDeclarationSyntax)Visit(member) };
        }
    }
}