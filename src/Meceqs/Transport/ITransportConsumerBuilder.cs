using System;
using Meceqs.Pipeline;
using Meceqs.TypedHandling;

namespace Meceqs.Transport
{
    public interface ITransportConsumerBuilder<TTransportConsumer>
        where TTransportConsumer : ITransportConsumerBuilder<TTransportConsumer>
    {
        TTransportConsumer Instance { get; }

        TTransportConsumer AddMessageType<TMessage>();

        TTransportConsumer AddMessageType<TMessage, TResult>();

        TTransportConsumer AddMessageType(Type messageType, Type resultType = null);

        TTransportConsumer SetUnknownMessageBehavior(UnknownMessageBehavior behavior);

        TTransportConsumer ThrowOnUnknownMessage();

        TTransportConsumer SkipUnknownMessages();

        TTransportConsumer SetPipelineName(string pipelineName);

        TTransportConsumer ConfigurePipeline(Action<IPipelineBuilder> pipeline);

        TTransportConsumer UseTypedHandling(Action<TypedHandlingOptions> options);

        TTransportConsumer UseTypedHandling(TypedHandlingOptions options);
    }
}