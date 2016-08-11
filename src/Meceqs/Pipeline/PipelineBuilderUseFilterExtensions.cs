using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding a typed filter.
    /// </summary>
    public static class PipelineBuilderUseFilterExtensions
    {
        private const string InvokeMethodName = "Invoke";

        private static readonly MethodInfo GetServiceInfo = typeof(PipelineBuilderUseFilterExtensions).GetMethod(nameof(GetService), BindingFlags.NonPublic | BindingFlags.Static);

        public static IPipelineBuilder UseFilter<TFilter>(this IPipelineBuilder builder, params object[] args)
        {
            return builder.UseFilter(typeof(TFilter), args);
        }

        public static IPipelineBuilder UseFilter(this IPipelineBuilder builder, Type filter, params object[] args)
        {
            var applicationServices = builder.ApplicationServices;
            return builder.Use(next =>
            {
                var methods = filter.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                var invokeMethods = methods.Where(m => string.Equals(m.Name, InvokeMethodName, StringComparison.Ordinal)).ToArray();
                if (invokeMethods.Length > 1)
                {
                    throw new InvalidOperationException($"Multiple public '{InvokeMethodName}' methods are available.");
                }

                if (invokeMethods.Length == 0)
                {
                    throw new InvalidOperationException($"No public '{InvokeMethodName}' method found.");
                }

                var methodinfo = invokeMethods[0];
                if (!typeof(Task).IsAssignableFrom(methodinfo.ReturnType))
                {
                    throw new InvalidOperationException($"'{InvokeMethodName}' does not return an object of type '{nameof(Task)}'.");
                }

                var parameters = methodinfo.GetParameters();
                if (parameters.Length == 0 || parameters[0].ParameterType != typeof(FilterContext))
                {
                    throw new InvalidOperationException($"The '{InvokeMethodName}' method's first argument must be of type '{nameof(FilterContext)}'.");
                }

                var ctorArgs = new object[args.Length + 1];
                ctorArgs[0] = next;
                Array.Copy(args, 0, ctorArgs, 1, args.Length);
                var instance = ActivatorUtilities.CreateInstance(builder.ApplicationServices, filter, ctorArgs);
                if (parameters.Length == 1)
                {
                    return (FilterDelegate)methodinfo.CreateDelegate(typeof(FilterDelegate), instance);
                }

                var factory = Compile<object>(methodinfo, parameters);

                return context =>
                {
                    var serviceProvider = context.RequestServices ?? applicationServices;
                    if (serviceProvider == null)
                    {
                        throw new InvalidOperationException($"'{nameof(IServiceProvider)}' is not available.");
                    }

                    return factory(instance, context, serviceProvider);
                };
            });
        }

        private static Func<T, FilterContext, IServiceProvider, Task> Compile<T>(MethodInfo methodinfo, ParameterInfo[] parameters)
        {
            var filterType = typeof(T);

            var filterContextArg = Expression.Parameter(typeof(FilterContext), "filterContext");
            var providerArg = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
            var instanceArg = Expression.Parameter(filterType, "filter");

            var methodArguments = new Expression[parameters.Length];
            methodArguments[0] = filterContextArg;
            for (int i = 1; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                if (parameterType.IsByRef)
                {
                    throw new NotSupportedException($"The '{0}' method must not have ref or out parameters.");
                }

                var parameterTypeExpression = new Expression[]
                {
                    providerArg,
                    Expression.Constant(parameterType, typeof(Type)),
                    Expression.Constant(methodinfo.DeclaringType, typeof(Type))
                };

                var getServiceCall = Expression.Call(GetServiceInfo, parameterTypeExpression);
                methodArguments[i] = Expression.Convert(getServiceCall, parameterType);
            }

            Expression filterInstanceArg = instanceArg;
            if (methodinfo.DeclaringType != typeof(T))
            {
                filterInstanceArg = Expression.Convert(filterInstanceArg, methodinfo.DeclaringType);
            }

            var body = Expression.Call(filterInstanceArg, methodinfo, methodArguments);

            var lambda = Expression.Lambda<Func<T, FilterContext, IServiceProvider, Task>>(body, instanceArg, filterContextArg, providerArg);

            return lambda.Compile();
        }

        private static object GetService(IServiceProvider serviceProvider, Type type, Type filterType)
        {
            var service = serviceProvider.GetService(type);
            if (service == null)
            {
                throw new InvalidOperationException($"Unable to resolve service for type '{type}' while attempting to Invoke filter '{filterType}'.");
            }

            return service;
        }
    }
}