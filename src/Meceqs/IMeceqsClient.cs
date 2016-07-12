using System;
using System.Threading.Tasks;
using Meceqs.Sending;

namespace Meceqs
{
    public interface IMeceqsClient
    {
        ISendBuilder<TMessage> CreateSender<TMessage>(TMessage message, Guid messageId)
            where TMessage : IMessage;

        Task<TResult> HandleAsync<TMessage, TResult>(Envelope<TMessage> envelope)
            where TMessage : IMessage;
    }
}