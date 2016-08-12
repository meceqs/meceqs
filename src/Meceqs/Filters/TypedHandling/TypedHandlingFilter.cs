using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meceqs.Filters.TypedHandling.Internal;
using Meceqs.Pipeline;

namespace Meceqs.Filters.TypedHandling
{
    public class TypedHandlingFilter
    {
        private readonly IHandlerFactory _handlerFactory;
        private readonly IHandlerFactoryInvoker _handlerFactoryInvoker;
        private readonly IHandleContextFactory _handleContextFactory;
        private readonly IHandleMethodResolver _handleMethodResolver;
        private readonly IEnumerable<IHandleInterceptor> _handleInterceptors;
        private readonly IHandlerInvoker _handlerInvoker;

        public TypedHandlingFilter(
            FilterDelegate next,
            IHandlerFactory handlerFactory,
            IHandlerFactoryInvoker handlerFactoryInvoker,
            IHandleContextFactory handleContextFactory,
            IHandleMethodResolver handleMethodResolver,
            IEnumerable<IHandleInterceptor> handleInterceptors,
            IHandlerInvoker handlerInvoker)
        {
            // next is not stored because this is a terminal filter!

            Check.NotNull(handlerFactory, nameof(handlerFactory));
            Check.NotNull(handlerFactoryInvoker, nameof(handlerFactoryInvoker));
            Check.NotNull(handleContextFactory, nameof(handleContextFactory));
            Check.NotNull(handleMethodResolver, nameof(handleMethodResolver));
            Check.NotNull(handlerInvoker, nameof(handlerInvoker));

            _handlerFactory = handlerFactory;
            _handlerFactoryInvoker = handlerFactoryInvoker;
            _handleContextFactory = handleContextFactory;
            _handleMethodResolver = handleMethodResolver;
            _handleInterceptors = handleInterceptors ?? Enumerable.Empty<IHandleInterceptor>();
            _handlerInvoker = handlerInvoker;
        }

        public async Task Invoke(FilterContext filterContext)
        {
            Check.NotNull(filterContext, nameof(filterContext));

            // Since the public interfaces from this filter expect generic types, we can't call them directly.
            // Separate services are responsible for invoking them by using e.g. reflection.

            Type messageType = filterContext.MessageType;
            Type resultType = filterContext.ExpectedResultType;

            IHandles handler = _handlerFactoryInvoker.InvokeCreateHandler(_handlerFactory, messageType, resultType);

            if (handler == null)
            {
                throw new InvalidOperationException($"No handler found for '{messageType.Name}/{resultType.Name}'");
            }

            HandleContext handleContext = _handleContextFactory.CreateHandleContext(filterContext);

            SetContextProperties(handleContext, handler, messageType, resultType);

            foreach (var interceptor in _handleInterceptors)
            {
                await interceptor.OnHandleExecuting(handleContext);
            }

            filterContext.Result = await _handlerInvoker.InvokeHandleAsync(handler, handleContext, resultType);

            foreach (var interceptor in _handleInterceptors.Reverse())
            {
                await interceptor.OnHandleExecuted(handleContext);
            }
        }

        private void SetContextProperties(HandleContext handleContext, IHandles handler, Type messageType, Type resultType)
        {
            var handlerType = handler.GetType();

            // This allows interceptors to e.g. look for custom attributes on the class or method.
            handleContext.Handler = handler;
            handleContext.HandlerType = handlerType;
            handleContext.HandleMethod = _handleMethodResolver.GetHandleMethod(handlerType, messageType, resultType);

            if (handleContext.HandleMethod == null)
            {
                throw new InvalidOperationException(
                    $"'{nameof(_handleMethodResolver.GetHandleMethod)}' " +
                    $"did not find a Handle-method for '{handlerType}.{messageType}/{resultType}'");
            }
        }
    }
}