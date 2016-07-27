using System;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Sending.TypedSend
{
    public class DefaultSenderFactory : ISenderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultSenderFactory(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public ISender<TMessage, TResult> CreateSender<TMessage, TResult>() where TMessage : IMessage
        {
            return _serviceProvider.GetRequiredService<ISender<TMessage, TResult>>();
        }
    }
}