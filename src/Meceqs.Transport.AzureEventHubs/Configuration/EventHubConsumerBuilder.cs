using System;
using System.Collections.Generic;
using System.Reflection;
using Meceqs.Configuration;
using Meceqs.Filters.TypedHandling;
using Meceqs.Filters.TypedHandling.Configuration;
using Meceqs.Pipeline;
using Meceqs.Transport.AzureEventHubs.Consuming;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Transport.AzureEventHubs.Configuration
{
    public class EventHubConsumerBuilder : IEventHubConsumerBuilder
    {
        private readonly List<Assembly> _deserializationAssemblies = new List<Assembly>();

        private Action<EventHubConsumerOptions> _consumerOptions;
        private Action<TypedHandlingOptions> _typedHandlingOptions;

        private string _pipelineName;
        private Action<IPipelineBuilder> _pipeline;

        public IEnumerable<Assembly> GetDeserializationAssemblies() => _deserializationAssemblies;
        public Action<EventHubConsumerOptions> GetConsumerOptions() => _consumerOptions;
        public Action<TypedHandlingOptions> GetTypedHandlingOptions() => _typedHandlingOptions;
        public string GetPipelineName() => _pipelineName;

        public Action<IPipelineBuilder> GetPipeline()
        {
            if (_typedHandlingOptions == null)
            {
                // No handler/interceptor was added so we assume the user wants to add a custom pipeline.
                if (_pipeline == null)
                {
                    throw new MeceqsException(
                        $"No pipeline was configured. You can either use TypedHandling by adding a " + 
                        $"typed handler via '{nameof(AddTypedHandler)}' or you can configure " + 
                        $"a custom pipeline by calling '{nameof(ConfigurePipeline)}'.");
                }

                return _pipeline;
            }

            // The user wants to use TypedHandling.
            // In this case, the pipeline is treated as filters that will be called 
            // BEFORE the TypedHandlingFilter.

            _pipeline += x => x.RunTypedHandling(_typedHandlingOptions);
            return _pipeline;
        }

        public IEventHubConsumerBuilder AddMessageType<TMessage>()
        {
            return AddMessageType(typeof(TMessage));
        }

        public IEventHubConsumerBuilder AddMessageType(Type messageType)
        {
            Check.NotNull(messageType, nameof(messageType));

            _consumerOptions += x => x.AddMessageType(messageType);

            // To be able to work with the message in the consumer,
            // we must also be able to deserialize it.
            if (!_deserializationAssemblies.Contains(messageType.Assembly))
            {
                _deserializationAssemblies.Add(messageType.Assembly);
            }

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

            // We have to immediately validate the type to be able to extract the message types.
            HandlerCollection.EnsureValidHandler(handlerType);

            _typedHandlingOptions += x => x.Handlers.Add(handlerType);

            // We must also tell the EventHubConsumer that it should accept the message types!
            foreach (var implementedHandle in HandlerCollection.GetImplementedHandles(handlerType))
            {
                AddMessageType(implementedHandle.Item1);
            }

            return this;
        }

        public IEventHubConsumerBuilder AddTypedHandleInterceptor<TInterceptor>()
            where TInterceptor : class, IHandleInterceptor
        {
            return AddTypedHandleInterceptor(typeof(TInterceptor));
        }

        public IEventHubConsumerBuilder AddTypedHandleInterceptor(Type interceptorType)
        {
            Check.NotNull(interceptorType, nameof(interceptorType));

            _typedHandlingOptions += x => x.Interceptors.Add(interceptorType);
            return this;
        }

        public IEventHubConsumerBuilder ConfigurePipeline(Action<IPipelineBuilder> pipeline)
        {
            return ConfigurePipeline(null, pipeline);
        }

        public IEventHubConsumerBuilder ConfigurePipeline(string pipelineName, Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            _pipelineName = pipelineName;
            _pipeline = pipeline;
            return this;
        }
    }
}