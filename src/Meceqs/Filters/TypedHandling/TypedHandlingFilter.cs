using System;
using System.Linq;
using System.Threading.Tasks;
using Meceqs.Filters.TypedHandling.Internal;
using Meceqs.Pipeline;

namespace Meceqs.Filters.TypedHandling
{
    public class TypedHandlingFilter
    {
        private readonly FilterDelegate _next;
        private readonly TypedHandlingOptions _options;
        private readonly IHandlerFactoryInvoker _handlerFactoryInvoker;
        private readonly IHandleContextFactory _handleContextFactory;
        private readonly IHandleMethodResolver _handleMethodResolver;
        private readonly IHandlerInvoker _handlerInvoker;

        public TypedHandlingFilter(
            FilterDelegate next,
            TypedHandlingOptions options,
            IHandlerFactory handlerFactory,
            IHandlerFactoryInvoker handlerFactoryInvoker,
            IHandleContextFactory handleContextFactory,
            IHandleMethodResolver handleMethodResolver,
            IHandlerInvoker handlerInvoker)
        {
            Check.NotNull(next, nameof(next));
            Check.NotNull(options, nameof(options));
            Check.NotNull(handlerFactoryInvoker, nameof(handlerFactoryInvoker));
            Check.NotNull(handleContextFactory, nameof(handleContextFactory));
            Check.NotNull(handleMethodResolver, nameof(handleMethodResolver));
            Check.NotNull(handlerInvoker, nameof(handlerInvoker));

            _next = next;
            _options = options;
            _handlerFactoryInvoker = handlerFactoryInvoker;
            _handleContextFactory = handleContextFactory;
            _handleMethodResolver = handleMethodResolver;
            _handlerInvoker = handlerInvoker;
        }

        public async Task Invoke(
            FilterContext filterContext,
            IHandlerFactory handlerFactory)
        {
            Check.NotNull(filterContext, nameof(filterContext));
            Check.NotNull(filterContext.RequestServices, $"{nameof(filterContext)}.{nameof(filterContext.RequestServices)}");
            Check.NotNull(handlerFactory, nameof(handlerFactory));

            // Since the public interfaces from this filter expect generic types, we can't call them directly.
            // Separate services are responsible for invoking them by using e.g. reflection.

            IHandles handler = _handlerFactoryInvoker.InvokeCreateHandler(
                handlerFactory,
                filterContext.MessageType,
                filterContext.ExpectedResultType);

            if (handler == null)
            {
                // let some other filter decide whether unhandled messages should throw or not.
                await _next(filterContext);
                return;
            }

            var handleContext = CreateHandleContext(filterContext, handler);

            var handleExecutionChain = CreateHandleExecutionChain(handleContext);

            await handleExecutionChain(handleContext);
        }

        private HandleContext CreateHandleContext(FilterContext filterContext, IHandles handler)
        {
            HandleContext handleContext = _handleContextFactory.CreateHandleContext(filterContext);

            var handlerType = handler.GetType();
            Type messageType = filterContext.MessageType;
            Type resultType = filterContext.ExpectedResultType;

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

            return handleContext;
        }

        private HandleExecutionDelegate CreateHandleExecutionChain(HandleContext handleContext)
        {
            // The call to handler itself is the most-inner call.
            HandleExecutionDelegate chain = async (HandleContext context) =>
            {
                context.FilterContext.Result = await _handlerInvoker.InvokeHandleAsync(
                    context.Handler,
                    context,
                    context.FilterContext.ExpectedResultType);
            };

            // Wrap every existing interceptor around this call.
            foreach (var metadata in _options.Interceptors.Reverse())
            {
                // This creates a delegate that contains a delegate :)
                // The outer delegate is used to create the chain.
                // The inner delegate represents the actual call to the interceptor and
                // is executed when the chain is executed.
                Func<HandleContext, HandleExecutionDelegate, HandleExecutionDelegate> interceptorFunc = (context, next) =>
                {
                    HandleExecutionDelegate interceptorCall = (HandleContext innerContext) =>
                    {
                        var interceptor = metadata.CreateInterceptor(innerContext.FilterContext.RequestServices);
                        return interceptor.OnHandleExecutionAsync(innerContext, next);
                    };
                    return interceptorCall;
                };

                chain = interceptorFunc(handleContext, chain);
            }

            return chain;
        }
    }
}