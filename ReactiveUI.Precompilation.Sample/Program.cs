using System;

namespace ReactiveUI.Precompilation.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var model = new TestModel();
            model.StringProperty = "foo";
            Console.WriteLine(model.WrapperProperty);
            Console.ReadLine();
        }
    }

    class TestModel : ReactiveObject
    {
        [Reactive]
        public string StringProperty { get; set; }

        [ObservableAsProperty]
        public string WrapperProperty { get; private set; }

        public TestModel()
        {
            this.WhenAnyValue(x => x.StringProperty).ToPropertyEx(this, x => x.WrapperProperty);
        }
    }
}
