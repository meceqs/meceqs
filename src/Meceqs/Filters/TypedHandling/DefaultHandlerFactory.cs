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

        public IHandles<TMessage> CreateHandler<TMessage>() where TMessage : class
        {
            return _serviceProvider.GetService<IHandles<TMessage>>();
        }

        public IHandles<TMessage, TResult> CreateHandler<TMessage, TResult>() where TMessage : class
        {
            return _serviceProvider.GetService<IHandles<TMessage, TResult>>();
        }
    }
}