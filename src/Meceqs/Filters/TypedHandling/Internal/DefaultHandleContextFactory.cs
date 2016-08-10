using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Meceqs.Pipeline;

namespace Meceqs.Filters.TypedHandling.Internal
{
    /// <summary>
    /// Creates a typed <see typeref="HandleContext" /> for the given <see typeref="FilterContext" />.
    /// For better performance, it uses cached delegates to create the instance.
    /// </summary>
    public class DefaultHandleContextFactory : IHandleContextFactory
    {
        private readonly ConcurrentDictionary<Type, Func<FilterContext, HandleContext>> _cachedConstructorDelegates;

        public DefaultHandleContextFactory()
        {
            _cachedConstructorDelegates = new ConcurrentDictionary<Type, Func<FilterContext, HandleContext>>();
        }

        public HandleContext CreateHandleContext(FilterContext filterContext)
        {
            Check.NotNull(filterContext, nameof(filterContext));

            Type messageType = filterContext.Message.GetType();

            var ctorDelegate = GetOrAddConstructorDelegate(messageType);

            HandleContext handleContext = ctorDelegate(filterContext);

            return handleContext;
        }

        private Func<FilterContext, HandleContext> GetOrAddConstructorDelegate(Type messageType)
        {
            var ctorDelegate = _cachedConstructorDelegates.GetOrAdd(messageType, x =>
            {
                // Resolve types
                Type typedHandleContextType = typeof(HandleContext<>).MakeGenericType(messageType);
                Type typedFilterContextType = typeof(FilterContext<>).MakeGenericType(messageType);

                ConstructorInfo constructor = typedHandleContextType.GetTypeInfo().DeclaredConstructors.First();

                // Create Expression

                // parameters for constructor
                var filterContextParam = Expression.Parameter(typeof(FilterContext), "filterContext");
                var castedFilterContextParam = Expression.Convert(filterContextParam, typedFilterContextType);

                // Create constructor call
                var compiledDelegate = Expression.Lambda<Func<FilterContext, HandleContext>>(
                    Expression.New(constructor, castedFilterContextParam),
                    filterContextParam
                ).Compile();

                return compiledDelegate;
            });

            return ctorDelegate;
        }
    }
}