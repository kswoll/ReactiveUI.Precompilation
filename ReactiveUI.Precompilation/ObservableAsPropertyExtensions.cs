using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reflection;

namespace ReactiveUI.Precompilation
{
    public static class ObservableAsPropertyExtensions
    {
        public static ObservableAsPropertyHelper<TRet> ToPropertyEx<TObj, TRet>(this IObservable<TRet> @this, TObj source, Expression<Func<TObj, TRet>> property, TRet initialValue = default(TRet), bool deferSubscription = false, IScheduler scheduler = null) where TObj : ReactiveObject
        {
            var result = @this.ToProperty(source, property, initialValue, deferSubscription, scheduler);

            // Now assign the field via reflection.
            var propertyInfo = property.GetPropertyInfo();
            if (propertyInfo == null)
                throw new ObservableAsPropertyException($"Could not resolve expression {property} into a property.");
            var field = propertyInfo.DeclaringType.GetTypeInfo().GetDeclaredField($"<{propertyInfo.Name}>k__BackingField");
            if (field == null)
                throw new ObservableAsPropertyException($"Backing field not found for {propertyInfo}");
            field.SetValue(source, result);

            return result;
        }

        private static PropertyInfo GetPropertyInfo(this LambdaExpression expression)
        {
            var current = expression.Body;
            var unary = current as UnaryExpression;
            if (unary != null)
                current = unary.Operand;
            var call = (MemberExpression)current;
            return (PropertyInfo)call.Member;
        }
    }
}