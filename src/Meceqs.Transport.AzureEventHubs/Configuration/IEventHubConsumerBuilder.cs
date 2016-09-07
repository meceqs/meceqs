using System;
using Meceqs.Configuration;
using Meceqs.Filters.TypedHandling;
using Meceqs.Pipeline;

namespace Meceqs.Transport.AzureEventHubs.Configuration
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