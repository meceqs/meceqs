using System;

namespace Meceqs.Handling
{
    public class DefaultHandlerResolver : IHandlerResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultHandlerResolver(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public IHandles<TMessage, TResult> Resolve<TMessage, TResult>() where TMessage : IMessage
        {
            var handler = (IHandles<TMessage, TResult>)_serviceProvider.GetService(typeof(IHandles<TMessage, TResult>));
            return handler;
        }
    }
}