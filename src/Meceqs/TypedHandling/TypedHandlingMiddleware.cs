using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Meceqs.Transport;
using Meceqs.TypedHandling.Configuration;
using Meceqs.TypedHandling.Internal;
using Microsoft.Extensions.Logging;

namespace Meceqs.TypedHandling
{
    public class TypedHandlingMiddleware
    {
        private readonly TypedHandlingOptions _options;
        private readonly IHandleMethodResolver _handleMethodResolver;
        private readonly ILogger _logger;

        private readonly Dictionary<HandleDefinition, IHandlerMetadata> _handlerMapping;
        private readonly HandleExecutionDelegate _handleExecutionChain;

        public TypedHandlingMiddleware(
            MiddlewareDelegate next,
            TypedHandlingOptions options,
            IHandleMethodResolver handleMethodResolver,
            IHandlerInvoker handlerInvoker,
            ILoggerFactory loggerFactory)
        {
            // "next" is not stored because this is a terminating middleware.

            Guard.NotNull(options, nameof(options));
            Guard.NotNull(handleMethodResolver, nameof(handleMethodResolver));
            Guard.NotNull(handlerInvoker, nameof(handlerInvoker));
            Guard.NotNull(loggerFactory, nameof(loggerFactory));

            _options = options;
            _handleMethodResolver = handleMethodResolver;
            _logger = loggerFactory.CreateLogger<TypedHandlingMiddleware>();

            _handlerMapping = CreateHandlerMapping(options.Handlers);
            _handleExecutionChain = CreateHandleExecutionChain(handlerInvoker, _options.Interceptors);
        }

        public async Task Invoke(MessageContext messageContext)
        {
            Guard.NotNull(messageContext, nameof(messageContext));

            // Since the public interfaces from this middleware expect generic types, we can't call them directly.
            // Separate services are responsible for invoking them by using e.g. reflection.

            IHandles handler = CreateHandler(messageContext);
            if (handler == null)
            {
                HandleUnknownMessageType(messageContext);
                return;
            }

            var handleContext = CreateHandleContext(messageContext, handler);

            await _handleExecutionChain(handleContext);
        }

        private IHandles CreateHandler(MessageContext messageContext)
        {
            var key = new HandleDefinition(messageContext.MessageType, messageContext.ExpectedResponseType);

            IHandlerMetadata handlerMetadata;
            if (_handlerMapping.TryGetValue(key, out handlerMetadata))
            {
                return handlerMetadata.CreateHandler(messageContext.RequestServices);
            }

            return null;
        }

        private void HandleUnknownMessageType(MessageContext messageContext)
        {
            switch (_options.UnknownMessageBehavior)
            {
                case UnknownMessageBehavior.ThrowException:
                    throw new UnknownMessageException(
                        $"There was no handler configured for message/response types " +
                        $"'{messageContext.MessageType}/{messageContext.ExpectedResponseType}");

                case UnknownMessageBehavior.Skip:
                    _logger.SkippingMessage(messageContext);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        $"options.{nameof(_options.UnknownMessageBehavior)}",
                        _options.UnknownMessageBehavior,
                        "The given value is not supported");
            }
        }

        private HandleContext CreateHandleContext(MessageContext messageContext, IHandles handler)
        {
            var handlerType = handler.GetType();
            var messageType = messageContext.MessageType;
            var responseType = messageContext.ExpectedResponseType;

            // This allows interceptors to e.g. look for custom attributes on the class or method.
            var handleMethod = _handleMethodResolver.GetHandleMethod(handlerType, messageType, responseType);

            if (handleMethod == null)
            {
                throw new InvalidOperationException(
                    $"'{nameof(_handleMethodResolver.GetHandleMethod)}' " +
                    $"did not find a Handle-method for '{handlerType}.{messageType}/{responseType}'");
            }

            HandleContext handleContext = new HandleContext(messageContext, handler, handleMethod);

            return handleContext;
        }

        private static HandleExecutionDelegate CreateHandleExecutionChain(IHandlerInvoker handlerInvoker, InterceptorCollection interceptors)
        {
            // The call to handler itself is the innermost call.
            HandleExecutionDelegate chain = (HandleContext context) =>
            {
                return handlerInvoker.InvokeHandleAsync(context);
            };

            // Wrap every existing interceptor around this call.
            foreach (var metadata in interceptors.Reverse())
            {
                // This creates a delegate that contains a delegate :)
                // The outer delegate is used to create the chain.
                // The inner delegate represents the actual call to the interceptor and
                // is executed when the chain is executed.
                Func<HandleExecutionDelegate, HandleExecutionDelegate> interceptorFunc = (next) =>
                {
                    HandleExecutionDelegate interceptorCall = (HandleContext innerContext) =>
                    {
                        var interceptor = metadata.CreateInterceptor(innerContext.RequestServices);
                        return interceptor.OnHandleExecutionAsync(innerContext, next);
                    };
                    return interceptorCall;
                };

                chain = interceptorFunc(chain);
            }

            return chain;
        }

        /// <summary>
        /// Returns a dictionary which returns a <see cref="IHandlerMetadata"/> for a given a message type and response type.
        /// </summary>
        private static Dictionary<HandleDefinition, IHandlerMetadata> CreateHandlerMapping(HandlerCollection handlers)
        {
            Guard.NotNull(handlers, nameof(handlers));

            if (handlers.Count == 0)
            {
                throw new MeceqsException(
                    $"The options don't contain any handlers. " +
                    $"Handlers can be added by calling '{nameof(HandlerCollection)}.{nameof(HandlerCollection.Add)}' " +
                    $"or '{nameof(HandlerCollection)}.{nameof(HandlerCollection.AddService)}'.");
            }

            var dictionary = new Dictionary<HandleDefinition, IHandlerMetadata>();

            foreach (var handlerMetadata in handlers)
            {
                foreach (var implementedHandle in handlerMetadata.ImplementedHandles)
                {
                    dictionary.Add(implementedHandle, handlerMetadata);
                }
            }

            return dictionary;
        }
    }
}
