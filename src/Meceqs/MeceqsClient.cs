using System;
using System.Threading.Tasks;
using Meceqs.Sending;

namespace Meceqs
{
    public class MeceqsClient : IMeceqsClient
    {
        private readonly MeceqsConfiguration _configuration;

        public MeceqsClient(MeceqsConfiguration configuration)
        {
            Check.NotNull(configuration, nameof(configuration));

            _configuration = configuration;
        }

        public ISendBuilder<TMessage> CreateSender<TMessage>(TMessage message, Guid messageId) where TMessage : IMessage
        {
            return _configuration.CreateMessageSender()
                .ForMessage(message, messageId);
        }

        public Task<TResult> HandleAsync<TMessage, TResult>(Envelope<TMessage> envelope) where TMessage : IMessage
        {
            //var envelopeHandler = new DefaultEnvelopeHandler(_configuration.HandlerFactory, );
            throw new NotImplementedException();
        }
    }
}