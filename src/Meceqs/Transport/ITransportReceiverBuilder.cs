using System;
using Meceqs.Pipeline;
using Meceqs.TypedHandling;

namespace Meceqs.Transport
{
    public interface ITransportReceiverBuilder<TTransportReceiver>
        where TTransportReceiver : ITransportReceiverBuilder<TTransportReceiver>
    {
        TTransportReceiver Instance { get; }

        TTransportReceiver AddMessageType<TMessage>();

        TTransportReceiver AddMessageType<TMessage, TResult>();

        TTransportReceiver AddMessageType(Type messageType, Type resultType = null);

        TTransportReceiver SetUnknownMessageBehavior(UnknownMessageBehavior behavior);

        TTransportReceiver ThrowOnUnknownMessage();

        TTransportReceiver SkipUnknownMessages();

        TTransportReceiver SetPipelineName(string pipelineName);

        TTransportReceiver ConfigurePipeline(Action<PipelineOptions> pipeline);

        TTransportReceiver UseTypedHandling(Action<TypedHandlingOptions> options);

        TTransportReceiver UseTypedHandling(TypedHandlingOptions options);
    }
}