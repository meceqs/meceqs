using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Meceqs
{
    /// <summary>
    /// Creates a typed MessageContext for the given envelope.
    /// For better performance, it uses cached delegates to create the instance.
    /// </summary>
    public class DefaultMessageContextFactory : IMessageContextFactory
    {
        private readonly ConcurrentDictionary<Type, Func<Envelope, MessageContextData, CancellationToken, MessageContext>> _cachedConstructorDelegates;

        public DefaultMessageContextFactory()
        {
            _cachedConstructorDelegates = new ConcurrentDictionary<Type, Func<Envelope, MessageContextData, CancellationToken, MessageContext>>();
        }

        public MessageContext Create(Envelope envelope, MessageContextData contextData, CancellationToken cancellation)
        {
            Check.NotNull(envelope, nameof(envelope));
            Check.NotNull(contextData, nameof(contextData));

            Type messageType = envelope.Message.GetType();

            var ctorDelegate = GetOrAddConstructorDelegate(messageType);

            MessageContext context = ctorDelegate(envelope, contextData, cancellation);
            
            return context;
        }

        private Func<Envelope, MessageContextData, CancellationToken, MessageContext> GetOrAddConstructorDelegate(Type messageType)
        {
            var ctorDelegate = _cachedConstructorDelegates.GetOrAdd(messageType, x =>
            {
                // Resolve types
                Type typedMessageContext = typeof(MessageContext<>).MakeGenericType(messageType);
                ConstructorInfo constructor = typedMessageContext.GetTypeInfo().DeclaredConstructors.First();
                Type typedEnvelopeType = typeof(Envelope<>).MakeGenericType(messageType);

                // Create Expression

                // parameters for constructor
                var envelopeParam = Expression.Parameter(typeof(Envelope), "envelope");
                var contextDataParam = Expression.Parameter(typeof(MessageContextData), "contextData");
                var cancellationParam = Expression.Parameter(typeof(CancellationToken), "cancellation");

                var castedEnvelopeParam = Expression.Convert(envelopeParam, typedEnvelopeType);

                // Create constructor call
                var compiledDelegate = Expression.Lambda<Func<Envelope, MessageContextData, CancellationToken, MessageContext>>(
                    Expression.New(constructor, castedEnvelopeParam, contextDataParam, cancellationParam),
                    envelopeParam, contextDataParam, cancellationParam
                ).Compile();

                return compiledDelegate;
            });

            return ctorDelegate;
        }
    }
}