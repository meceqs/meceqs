using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Meceqs.Sending.Internal
{
    public class DefaultEnvelopeFactory : IEnvelopeFactory
    {
        private readonly ConcurrentDictionary<Type, Func<IMessage, Guid, Envelope>> _cachedConstructorDelegates;

        public DefaultEnvelopeFactory()
        {
            _cachedConstructorDelegates = new ConcurrentDictionary<Type, Func<IMessage, Guid, Envelope>>();
        }

        public Envelope Create(IMessage message, Guid messageId)
        {
            Check.NotNull(message, nameof(message));
            Check.NotEmpty(messageId, nameof(messageId));

            Type messageType = message.GetType();

            var ctorDelegate = GetOrAddConstructorDelegate(messageType);

            Envelope envelope = ctorDelegate(message, messageId);

            return envelope;
        }

        private Func<IMessage, Guid, Envelope> GetOrAddConstructorDelegate(Type messageType)
        {
            var ctorDelegate = _cachedConstructorDelegates.GetOrAdd(messageType, x =>
            {
                // Resolve types
                Type typedEnvelopeType = typeof(Envelope<>).MakeGenericType(messageType);
                
                // select non-default constructor
                ConstructorInfo constructor = typedEnvelopeType.GetTypeInfo().DeclaredConstructors
                    .FirstOrDefault(ctor => ctor.GetParameters().Length > 0);

                // Create Expression

                // parameters for constructor
                var messageParam = Expression.Parameter(typeof(IMessage), "message");
                var castedMessageParam = Expression.Convert(messageParam, messageType);
                var messageIdParam = Expression.Parameter(typeof(Guid), "messageId");

                // Create constructor call
                var compiledDelegate = Expression.Lambda<Func<IMessage, Guid, Envelope>>(
                    Expression.New(constructor, castedMessageParam, messageIdParam),
                    messageParam, messageIdParam
                ).Compile();

                return compiledDelegate;
            });

            return ctorDelegate;
        }
    }
}