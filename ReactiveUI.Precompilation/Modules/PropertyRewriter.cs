using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StackExchange.Precompilation;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ReactiveUI.Precompilation.Modules
{
    public abstract class PropertyRewriter : CSharpSyntaxVisitor<IEnumerable<MemberDeclarationSyntax>>
    {
        public abstract IEnumerable<MemberDeclarationSyntax> RewriteProperty(PropertyDeclarationSyntax property);

        protected readonly BeforeCompileContext context;
        protected readonly INamedTypeSymbol attribute;
        protected readonly Compilation compilation;
        protected readonly AttributeListSyntax compilerGeneratedAttribute;
        protected readonly SyntaxTokenList privateAccess;

        protected PropertyRewriter(BeforeCompileContext context, INamedTypeSymbol attribute)
        {
            this.context = context;
            this.attribute = attribute;
            compilation = context.Compilation;
            compilerGeneratedAttribute = AttributeList(SeparatedList(new[] { Attribute(ParseName("System.Runtime.CompilerServices.CompilerGeneratedAttribute")) }));
            privateAccess = TokenList(Token(SyntaxKind.PrivateKeyword));
        }

        public bool IsApplicable(IPropertySymbol property)
        {
            return property.GetAttributes().Any(x => x.AttributeClass.IsEqualTo(attribute));
        }
    }
}
