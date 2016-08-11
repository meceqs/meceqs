using System;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Filters.TypedHandling
{
    public class DefaultHandlerFactory : IHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultHandlerFactory(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public IHandles<TMessage> CreateHandler<TMessage>() where TMessage : IMessage
        {
            return _serviceProvider.GetRequiredService<IHandles<TMessage>>();
        }

        public IHandles<TMessage, TResult> CreateHandler<TMessage, TResult>() where TMessage : IMessage
        {
            return _serviceProvider.GetRequiredService<IHandles<TMessage, TResult>>();
        }
    }
}