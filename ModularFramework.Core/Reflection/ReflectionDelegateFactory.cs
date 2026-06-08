using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ModularFramework.Core.Reflection
{
    public static class ReflectionDelegateFactory
    {
        public static Action<object, object> CreateSetter(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");
            var castInstance = Expression.Convert(instance, property.DeclaringType!);
            var castValue = Expression.Convert(value, property.PropertyType);
            var body = Expression.Call(castInstance, property.SetMethod!, castValue);
            return Expression.Lambda<Action<object, object>>(body, instance, value).Compile();
        }

        public static Func<object, object> CreateGetter(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var castInstance = Expression.Convert(instance, property.DeclaringType!);
            var body = Expression.Property(castInstance, property);
            var convert = Expression.Convert(body, typeof(object));
            return Expression.Lambda<Func<object, object>>(convert, instance).Compile();
        }

        public static Action<object, object[]> CreateMethodInvoker(MethodInfo method)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var parameters = Expression.Parameter(typeof(object[]), "args");
            var castInstance = Expression.Convert(instance, method.DeclaringType!);
            var paramExprs = method.GetParameters()
                .Select((p, i) =>
                    Expression.Convert(
                        Expression.ArrayIndex(parameters, Expression.Constant(i)),
                        p.ParameterType))
                .ToArray();
            var call = Expression.Call(castInstance, method, paramExprs);
            return Expression.Lambda<Action<object, object[]>>(call, instance, parameters).Compile();
        }
    }
}