using System;

namespace ReactiveUI.Precompilation
{
    public class ObservableAsPropertyException : Exception
    {
        public ObservableAsPropertyException(string message) : base(message)
        {
        }
    }
}