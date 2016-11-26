using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Meceqs.Pipeline;

namespace Meceqs.TypedHandling.Internal
{
    /// <summary>
    /// Creates a typed <see typeref="HandleContext" /> for the given <see typeref="MessageContext" />.
    /// For better performance, it uses cached delegates to create the instance.
    /// </summary>
    public class DefaultHandleContextFactory : IHandleContextFactory
    {
        private readonly ConcurrentDictionary<Type, Func<MessageContext, HandleContext>> _cachedConstructorDelegates;

        public DefaultHandleContextFactory()
        {
            _cachedConstructorDelegates = new ConcurrentDictionary<Type, Func<MessageContext, HandleContext>>();
        }

        public HandleContext CreateHandleContext(MessageContext messageContext)
        {
            Check.NotNull(messageContext, nameof(messageContext));

            Type messageType = messageContext.MessageType;

            var ctorDelegate = GetOrAddConstructorDelegate(messageType);

            HandleContext handleContext = ctorDelegate(messageContext);

            return handleContext;
        }

        private Func<MessageContext, HandleContext> GetOrAddConstructorDelegate(Type messageType)
        {
            var ctorDelegate = _cachedConstructorDelegates.GetOrAdd(messageType, x =>
            {
                // Resolve types
                Type typedHandleContextType = typeof(HandleContext<>).MakeGenericType(messageType);
                Type typedMessageContextType = typeof(MessageContext<>).MakeGenericType(messageType);

                ConstructorInfo constructor = typedHandleContextType.GetTypeInfo().DeclaredConstructors.First();

                // Create Expression

                // parameters for constructor
                var messageContextParam = Expression.Parameter(typeof(MessageContext), "messageContext");
                var castedMessageContextParam = Expression.Convert(messageContextParam, typedMessageContextType);

                // Create constructor call
                var compiledDelegate = Expression.Lambda<Func<MessageContext, HandleContext>>(
                    Expression.New(constructor, castedMessageContextParam),
                    messageContextParam
                ).Compile();

                return compiledDelegate;
            });

            return ctorDelegate;
        }
    }
}