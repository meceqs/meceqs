using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding a typed middleware.
    /// </summary>
    public static class PipelineOptionsUseMiddlewareExtensions
    {
        private const string InvokeMethodName = "Invoke";

        private static readonly MethodInfo GetServiceInfo = typeof(PipelineOptionsUseMiddlewareExtensions).GetMethod(nameof(GetService), BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// Adds a middleware class to the pipeline.
        /// </summary>
        public static PipelineOptions UseMiddleware<TMiddleware>(this PipelineOptions builder, params object[] args)
        {
            return builder.UseMiddleware(typeof(TMiddleware), args);
        }

        /// <summary>
        /// Adds a middleware class to the pipeline.
        /// </summary>
        public static PipelineOptions UseMiddleware(this PipelineOptions builder, Type middleware, params object[] args)
        {
            return builder.Use((next, applicationServices) =>
            {
                var methods = middleware.GetMethods(BindingFlags.Instance | BindingFlags.Public);
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
                if (parameters.Length == 0 || parameters[0].ParameterType != typeof(MessageContext))
                {
                    throw new InvalidOperationException($"The '{InvokeMethodName}' method's first argument must be of type '{nameof(MessageContext)}'.");
                }

                var ctorArgs = new object[args.Length + 1];
                ctorArgs[0] = next;
                Array.Copy(args, 0, ctorArgs, 1, args.Length);
                var instance = ActivatorUtilities.CreateInstance(applicationServices, middleware, ctorArgs);
                if (parameters.Length == 1)
                {
                    return (MiddlewareDelegate)methodinfo.CreateDelegate(typeof(MiddlewareDelegate), instance);
                }

                var factory = Compile<object>(methodinfo, parameters);

                return context =>
                {
                    if (context.RequestServices == null)
                    {
                        throw new InvalidOperationException($"'{nameof(IServiceProvider)}' is not available.");
                    }

                    return factory(instance, context, context.RequestServices);
                };
            });
        }

        private static Func<T, MessageContext, IServiceProvider, Task> Compile<T>(MethodInfo methodinfo, ParameterInfo[] parameters)
        {
            var middlewareType = typeof(T);

            var messageContextArg = Expression.Parameter(typeof(MessageContext), "messageContext");
            var providerArg = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
            var instanceArg = Expression.Parameter(middlewareType, "middleware");

            var methodArguments = new Expression[parameters.Length];
            methodArguments[0] = messageContextArg;
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

            Expression middlewareInstanceArg = instanceArg;
            if (methodinfo.DeclaringType != typeof(T))
            {
                middlewareInstanceArg = Expression.Convert(middlewareInstanceArg, methodinfo.DeclaringType);
            }

            var body = Expression.Call(middlewareInstanceArg, methodinfo, methodArguments);

            var lambda = Expression.Lambda<Func<T, MessageContext, IServiceProvider, Task>>(body, instanceArg, messageContextArg, providerArg);

            return lambda.Compile();
        }

        private static object GetService(IServiceProvider serviceProvider, Type type, Type middlewareType)
        {
            var service = serviceProvider.GetService(type);
            if (service == null)
            {
                throw new InvalidOperationException($"Unable to resolve service for type '{type}' while attempting to Invoke middleware '{middlewareType}'.");
            }

            return service;
        }
    }
}