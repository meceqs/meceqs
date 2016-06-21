using System;

namespace Meceqs.Consuming
{
    public class DefaultConsumerResolver : IConsumerResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultConsumerResolver(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public IConsumes<TMessage, TResult> Resolve<TMessage, TResult>() where TMessage : IMessage
        {
            var consumer = (IConsumes<TMessage, TResult>)_serviceProvider.GetService(typeof(IConsumes<TMessage, TResult>));
            return consumer;
        }
    }
}