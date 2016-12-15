using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StackExchange.Precompilation;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ReactiveUI.Precompilation.Modules
{
    public class ObservableAsPropertyRewriter : PropertyRewriter
    {
        public ObservableAsPropertyRewriter(BeforeCompileContext context, INamedTypeSymbol attribute) : base(context, attribute)
        {
        }

        public override IEnumerable<MemberDeclarationSyntax> RewriteProperty(PropertyDeclarationSyntax property)
        {
            // Declare a new field to store the property value
            var fieldName = Identifier($"<{property.Identifier}>k__BackingField");
            var observableAsPropertyHelper = ParseTypeName($"ReactiveUI.ObservableAsPropertyHelper<{property.Type}>");
            var field = FieldDeclaration(
                List(new[] { compilerGeneratedAttribute }),
                privateAccess,
                VariableDeclaration(
                    observableAsPropertyHelper,
                    SeparatedList<VariableDeclaratorSyntax>(
                        NodeOrTokenList(VariableDeclarator(fieldName))
                    )
                )
            );

            // Implement the getter
            var getter = AccessorDeclaration(
                SyntaxKind.GetAccessorDeclaration,
                List(new[] { compilerGeneratedAttribute }),
                property.AccessorList.Accessors.Single(x => x.Kind() == SyntaxKind.GetAccessorDeclaration).Modifiers,
                Block(
                    ReturnStatement(ConditionalAccessExpression(IdentifierName(fieldName), MemberBindingExpression(IdentifierName("Value"))))
                )
            );

            var setter = AccessorDeclaration(
                SyntaxKind.SetAccessorDeclaration,
                List(new[] { compilerGeneratedAttribute }),
                property.AccessorList.Accessors.Single(x => x.Kind() == SyntaxKind.SetAccessorDeclaration).Modifiers,
                Block(
                    ThrowStatement(ObjectCreationExpression(
                        ParseTypeName("ReactiveUI.Precompilation.ObservableAsPropertyException"),
                        ArgumentList(SeparatedList(new[]
                        {
                            Argument(LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                Literal("Never call the setter of an ObservabeAsPropertyHelper property.")
                            ))
                        })),
                        null
                    ))
                )
            );

            // Replace the accessors of the property with our new implementations
            var newProperty = property
                .WithAccessorList(AccessorList(
                    List(new[] { getter, setter })
                ));

            yield return field;
            yield return newProperty;
        }
    }
}