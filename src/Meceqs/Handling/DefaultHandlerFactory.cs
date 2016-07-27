using System;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Handling
{
    public class DefaultHandlerFactory : IHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultHandlerFactory(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public IHandler<TMessage, TResult> CreateHandler<TMessage, TResult>() where TMessage : IMessage
        {
            return _serviceProvider.GetRequiredService<IHandler<TMessage, TResult>>();
        }
    }
}