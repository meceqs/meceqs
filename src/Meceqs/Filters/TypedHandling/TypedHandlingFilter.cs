using System;
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
        private readonly IHandlerInvoker _handlerInvoker;

        public TypedHandlingFilter(
            FilterDelegate next,
            IHandlerFactory handlerFactory,
            IHandlerFactoryInvoker handlerFactoryInvoker,
            IHandleContextFactory handleContextFactory,
            IHandlerInvoker handlerInvoker)
        {
            // next is not stored because this is a terminal filter!

            Check.NotNull(handlerFactory, nameof(handlerFactory));
            Check.NotNull(handlerFactoryInvoker, nameof(handlerFactoryInvoker));
            Check.NotNull(handleContextFactory, nameof(handleContextFactory));
            Check.NotNull(handlerInvoker, nameof(handlerInvoker));

            _handlerFactory = handlerFactory;
            _handlerFactoryInvoker = handlerFactoryInvoker;
            _handleContextFactory = handleContextFactory;
            _handlerInvoker = handlerInvoker;
        }

        public async Task Invoke(FilterContext filterContext)
        {
            Check.NotNull(filterContext, nameof(filterContext));
            Check.NotNull(filterContext.ExpectedResultType, $"{nameof(filterContext)}.{nameof(filterContext.ExpectedResultType)}");

            // IHandlerFactory and IHandler expect generic types so we have to use reflection.
            // The calls are outsourced to separate invokers to make sure that they 
            // can be optimized independently.

            Type messageType = filterContext.Message.GetType();
            Type resultType = filterContext.ExpectedResultType;

            object handler = _handlerFactoryInvoker.InvokeCreateHandler(_handlerFactory, messageType, resultType);

            if (handler == null)
            {
                throw new InvalidOperationException($"No handler found for '{messageType.Name}/{resultType.Name}'");
            }

            HandleContext handleContext = _handleContextFactory.CreateHandleContext(filterContext);

            filterContext.Result = await _handlerInvoker.InvokeHandleAsync(handler, handleContext, resultType);
        }
    }
}