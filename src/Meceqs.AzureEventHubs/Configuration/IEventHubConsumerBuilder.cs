using System;
using Meceqs.Configuration;
using Meceqs.Pipeline;
using Meceqs.TypedHandling;

namespace Meceqs.AzureEventHubs.Configuration
{
    public interface IEventHubConsumerBuilder
    {
        IEventHubConsumerBuilder AddMessageType<TMessage>();

        IEventHubConsumerBuilder AddMessageType(Type messageType);

        IEventHubConsumerBuilder SetUnknownMessageBehavior(UnknownMessageBehavior behavior);

        IEventHubConsumerBuilder ThrowOnUnknownMessage();

        IEventHubConsumerBuilder SkipUnknownMessages();

        IEventHubConsumerBuilder ConfigurePipeline(Action<IPipelineBuilder> pipeline);

        IEventHubConsumerBuilder ConfigurePipeline(string pipelineName, Action<IPipelineBuilder> pipeline);

        IEventHubConsumerBuilder AddTypedHandler<THandler>()
           where THandler : class, IHandles;

        IEventHubConsumerBuilder AddTypedHandler(Type handlerType);

        IEventHubConsumerBuilder AddTypedHandleInterceptor<TInterceptor>()
            where TInterceptor : class, IHandleInterceptor;

        IEventHubConsumerBuilder AddTypedHandleInterceptor(Type interceptorType);
    }
}