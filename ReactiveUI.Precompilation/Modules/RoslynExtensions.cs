using System.Linq;
using Microsoft.CodeAnalysis;

namespace ReactiveUI.Precompilation.Modules
{
    public static class RoslynExtensions
    {
        public static bool IsAssignableFrom(this ITypeSymbol baseType, ITypeSymbol type)
        {
            var current = type;
            while (current != null)
            {
                if (Equals(current, baseType))
                    return true;
                current = current.BaseType;
            }
            foreach (var intf in type.AllInterfaces)
            {
                if (Equals(intf, baseType))
                    return true;
            }
            return false;
        }

        public static string GetFullName(this INamespaceSymbol namespaceSymbol)
        {
            string result = namespaceSymbol.MetadataName;
            if (!namespaceSymbol.IsGlobalNamespace && !namespaceSymbol.ContainingNamespace.IsGlobalNamespace)
                result = namespaceSymbol.ContainingNamespace.GetFullName() + "." + result;
            return result;
        }

        public static string GetFullName(this INamedTypeSymbol type)
        {
            string result = type.MetadataName;
            if (type.ContainingType != null)
                result = type.ContainingType.GetFullName() + "." + result;
            else if (!type.ContainingNamespace.IsGlobalNamespace)
                result = type.ContainingNamespace.GetFullName() + "." + result;
            return result;
        }

        public static bool IsEqualTo(this INamedTypeSymbol left, INamedTypeSymbol right)
        {
            return left.GetFullName().Equals(right.GetFullName());
        }

        public static bool HasAttribute(this ISymbol symbol, params INamedTypeSymbol[] attributeTypes)
        {
            return symbol.GetAttributes().Any(x => attributeTypes.Any(y => x.AttributeClass.IsEqualTo(y)));
        }
/*

        public static INamedTypeSymbol FindType(this Compilation compilation, string fullName)
        {
            var result = compilation.GetTypeByMetadataName(fullName);
            if (result == null)
            {
                foreach (IAssemblySymbol assembly in compilation.Met.MetadataReferences.Select(x => compilation.GetAssemblyOrModuleSymbol(x)))
                {
                    result = assembly.GetTypeByMetadataName(fullName);
                    if (result != null)
                        break;
                }
            }
            return result;
        }
*/
    }
}