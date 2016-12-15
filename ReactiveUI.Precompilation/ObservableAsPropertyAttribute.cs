using System;

namespace ReactiveUI.Precompilation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class ObservableAsPropertyAttribute : Attribute
    {
    }
}