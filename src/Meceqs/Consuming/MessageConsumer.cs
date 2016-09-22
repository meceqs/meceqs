using System;

namespace Meceqs.Consuming
{
    public class MessageConsumer : IMessageConsumer
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageConsumer(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public IFluentConsumer ForEnvelope(Envelope envelope)
        {
            Check.NotNull(envelope, nameof(envelope));

            return new FluentConsumer(envelope, _serviceProvider);
        }
    }
}