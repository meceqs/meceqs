using System;
using System.Collections.Generic;
using Meceqs.Filters.TypedHandling;
using Meceqs.Pipeline;
using Meceqs.Transport.AzureEventHubs.Consuming;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Transport.AzureEventHubs.Configuration
{
    public class EventHubConsumerBuilder : IEventHubConsumerBuilder
    {
        private readonly List<Type> _typedHandlers = new List<Type>();

        private Action<EventHubConsumerOptions> _consumerOptions;

        private string _pipelineName;
        private Action<IPipelineBuilder> _pipeline;

        public EventHubConsumerBuilder()
        {
            UseDefaultTypedHandlingPipeline();
        }

        public IEnumerable<Type> GetTypedHandlers() => _typedHandlers;
        public Action<EventHubConsumerOptions> GetConsumerOptions() => _consumerOptions;
        public string GetPipelineName() => _pipelineName;
        public Action<IPipelineBuilder> GetPipeline() => _pipeline;

        public IEventHubConsumerBuilder AddMessageType<TMessage>()
        {
            return AddMessageType(typeof(TMessage));
        }

        public IEventHubConsumerBuilder AddMessageType(Type messageType)
        {
            Check.NotNull(messageType, nameof(messageType));

            return AddMessageType(messageType.FullName);
        }

        public IEventHubConsumerBuilder AddMessageType(string messageType)
        {
            Check.NotNullOrWhiteSpace(messageType, nameof(messageType));

            _consumerOptions += x => x.AddMessageType(messageType);
            return this;
        }

        public IEventHubConsumerBuilder SetUnknownMessageBehavior(UnknownMessageBehavior behavior)
        {
            _consumerOptions += x => x.UnknownMessageBehavior = behavior;
            return this;
        }

        public IEventHubConsumerBuilder ThrowOnUnknownMessage()
        {
            return SetUnknownMessageBehavior(UnknownMessageBehavior.ThrowException);
        }

        public IEventHubConsumerBuilder SkipUnknownMessages()
        {
            return SetUnknownMessageBehavior(UnknownMessageBehavior.Skip);
        }

        public IEventHubConsumerBuilder AddTypedHandler<THandler>()
            where THandler : class, IHandles
        {
            return AddTypedHandler(typeof(THandler));
        }

        public IEventHubConsumerBuilder AddTypedHandler(Type handlerType)
        {
            Check.NotNull(handlerType, nameof(handlerType));

            if (!typeof(IHandles).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException(
                    $"Type '{handlerType}' must derive from '{typeof(IHandles)}'",
                    nameof(handlerType));
            }

            _typedHandlers.Add(handlerType);
            return this;
        }

        public IEventHubConsumerBuilder UsePipeline(Action<IPipelineBuilder> pipeline)
        {
            return UsePipeline(null, pipeline);
        }

        public IEventHubConsumerBuilder UsePipeline(string pipelineName, Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            _pipelineName = pipelineName;
            _pipeline = pipeline;
            return this;
        }

        public IEventHubConsumerBuilder UseDefaultTypedHandlingPipeline(Action<TypedHandlingOptions> options = null)
        {
            _pipeline = pipeline =>
            {
                pipeline.UseTypedHandling(options);
            };
            return this;
        }
    }
}