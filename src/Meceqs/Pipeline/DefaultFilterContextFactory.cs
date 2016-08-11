using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Creates a typed <see typeref="FilterContext" /> for the given <see typeref="Envelope" />.
    /// For better performance, it uses cached delegates to create the instance.
    /// </summary>
    public class DefaultFilterContextFactory : IFilterContextFactory
    {
        private readonly ConcurrentDictionary<Type, Func<Envelope, FilterContext>> _cachedConstructorDelegates;

        public DefaultFilterContextFactory()
        {
            _cachedConstructorDelegates = new ConcurrentDictionary<Type, Func<Envelope, FilterContext>>();
        }

        public FilterContext CreateFilterContext(Envelope envelope)
        {
            Check.NotNull(envelope, nameof(envelope));

            Type messageType = envelope.Message.GetType();

            var ctorDelegate = GetOrAddConstructorDelegate(messageType);

            FilterContext filterContext = ctorDelegate(envelope);

            return filterContext;
        }

        private Func<Envelope, FilterContext> GetOrAddConstructorDelegate(Type messageType)
        {
            var ctorDelegate = _cachedConstructorDelegates.GetOrAdd(messageType, x =>
            {
                // Resolve types
                Type typedEnvelopeType = typeof(Envelope<>).MakeGenericType(x);
                Type typedFilterContextType = typeof(FilterContext<>).MakeGenericType(x);
                
                ConstructorInfo constructor = typedFilterContextType.GetTypeInfo().DeclaredConstructors.First();

                // Create Expression

                // parameters for constructor
                var envelopeParam = Expression.Parameter(typeof(Envelope), "envelope");
                var castedEnvelopeParam = Expression.Convert(envelopeParam, typedEnvelopeType);

                // Create constructor call
                var compiledDelegate = Expression.Lambda<Func<Envelope, FilterContext>>(
                    Expression.New(constructor, castedEnvelopeParam),
                    envelopeParam
                ).Compile();

                return compiledDelegate;
            });

            return ctorDelegate;
        }
    }
}