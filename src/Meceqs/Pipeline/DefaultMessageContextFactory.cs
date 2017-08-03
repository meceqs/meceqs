using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Creates a typed <see typeref="MessageContext" /> for the given <see typeref="Envelope" />.
    /// For better performance, it uses cached delegates to create the instance.
    /// </summary>
    public class DefaultMessageContextFactory : IMessageContextFactory
    {
        private readonly ConcurrentDictionary<Type, Func<Envelope, MessageContext>> _cachedConstructorDelegates;

        public DefaultMessageContextFactory()
        {
            _cachedConstructorDelegates = new ConcurrentDictionary<Type, Func<Envelope, MessageContext>>();
        }

        public MessageContext CreateMessageContext(Envelope envelope)
        {
            Guard.NotNull(envelope, nameof(envelope));

            Type messageType = envelope.Message.GetType();

            var ctorDelegate = GetOrAddConstructorDelegate(messageType);

            MessageContext messageContext = ctorDelegate(envelope);

            return messageContext;
        }

        private Func<Envelope, MessageContext> GetOrAddConstructorDelegate(Type messageType)
        {
            var ctorDelegate = _cachedConstructorDelegates.GetOrAdd(messageType, x =>
            {
                // Resolve types
                Type typedEnvelopeType = typeof(Envelope<>).MakeGenericType(x);
                Type typedMessageContextType = typeof(MessageContext<>).MakeGenericType(x);

                ConstructorInfo constructor = typedMessageContextType.GetTypeInfo().DeclaredConstructors.First();

                // Create Expression

                // parameters for constructor
                var envelopeParam = Expression.Parameter(typeof(Envelope), "envelope");
                var castedEnvelopeParam = Expression.Convert(envelopeParam, typedEnvelopeType);

                // Create constructor call
                var compiledDelegate = Expression.Lambda<Func<Envelope, MessageContext>>(
                    Expression.New(constructor, castedEnvelopeParam),
                    envelopeParam
                ).Compile();

                return compiledDelegate;
            });

            return ctorDelegate;
        }
    }
}