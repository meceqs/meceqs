using System;
using Meceqs.Filters.TypedHandling;
using Meceqs.Pipeline;
using Meceqs.Transport.AzureEventHubs.Consuming;

namespace Meceqs.Transport.AzureEventHubs.Configuration
{
    public interface IEventHubConsumerBuilder
    {
        IEventHubConsumerBuilder AddMessageType<TMessage>();

        IEventHubConsumerBuilder AddMessageType(Type messageType);

        IEventHubConsumerBuilder AddMessageType(string messageType);

        IEventHubConsumerBuilder SetUnknownMessageBehavior(UnknownMessageBehavior behavior);

        IEventHubConsumerBuilder ThrowOnUnknownMessage();

        IEventHubConsumerBuilder SkipUnknownMessages();

        IEventHubConsumerBuilder UsePipeline(Action<IPipelineBuilder> pipeline);

        IEventHubConsumerBuilder UsePipeline(string pipelineName, Action<IPipelineBuilder> pipeline);

        IEventHubConsumerBuilder AddTypedHandler<THandler>()
           where THandler : class, IHandles;

        IEventHubConsumerBuilder AddTypedHandler(Type handlerType);

        IEventHubConsumerBuilder UseDefaultTypedHandlingPipeline(Action<TypedHandlingOptions> options = null);
    }
}