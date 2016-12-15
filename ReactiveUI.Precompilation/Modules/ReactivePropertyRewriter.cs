using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StackExchange.Precompilation;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ReactiveUI.Precompilation.Modules
{
    public class ReactivePropertyRewriter : PropertyRewriter
    {
        public ReactivePropertyRewriter(BeforeCompileContext context, INamedTypeSymbol attribute) : base(context, attribute)
        {
        }

        public override IEnumerable<MemberDeclarationSyntax> RewriteProperty(PropertyDeclarationSyntax property)
        {
            if (property.AccessorList.Accessors.Count != 2)
            {
                context.AddDiagnostic("Rx0001", "Missing accessors", "A [Reactive] property must have a getter and a setter.",
                    property.Identifier.GetLocation());
                yield return property;
            }

            // Declare a new field to store the property value
            var fieldName = Identifier($"<{property.Identifier}>k__BackingField");
            var field = FieldDeclaration(
                List(new[] { compilerGeneratedAttribute }),
                privateAccess,
                VariableDeclaration(
                    property.Type,
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
                    ReturnStatement(IdentifierName(fieldName))
                )
            );

            // Implement the setter
            var arguments = new[]
            {
                Argument(ThisExpression()),
                Argument(IdentifierName(fieldName)).WithRefOrOutKeyword(Token(SyntaxKind.RefKeyword)),
                Argument(IdentifierName("value")),
                Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(property.Identifier.ToString())))
            };
            var setter = AccessorDeclaration(
                SyntaxKind.SetAccessorDeclaration,
                List(new[] { compilerGeneratedAttribute }),
                property.AccessorList.Accessors.Single(x => x.Kind() == SyntaxKind.SetAccessorDeclaration).Modifiers,
                Block(
                    ExpressionStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("ReactiveUI"), IdentifierName("IReactiveObjectExtensions")),
                                IdentifierName("RaiseAndSetIfChanged")
                            ),
                            ArgumentList(SeparatedList(arguments, arguments.Skip(1).Select(x => Token(SyntaxKind.CommaToken))))
                        )
                    )
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