using System.Linq;
using Microsoft.CodeAnalysis;
using StackExchange.Precompilation;

namespace ReactiveUI.Precompilation.Modules
{
    public static class ContextExtensions
    {
        public static void AddDiagnostic(this BeforeCompileContext context, string code, string title, string messageFormat,
            Location location = null, DiagnosticSeverity severity = DiagnosticSeverity.Error)
        {
            context.Diagnostics.Add(Diagnostic.Create(
                new DiagnosticDescriptor(code, title, messageFormat, "ReactiveUI.Precompilation", severity, true),
                location ?? Location.None));
        }

        public static void AddDiagnostic(this AfterCompileContext context, string code, string title, string messageFormat,
            Location location = null, DiagnosticSeverity severity = DiagnosticSeverity.Error)
        {
            context.Diagnostics.Add(Diagnostic.Create(
                new DiagnosticDescriptor(code, title, messageFormat, "ReactiveUI.Precompilation", severity, true),
                location ?? Location.None));
        }
    }
}