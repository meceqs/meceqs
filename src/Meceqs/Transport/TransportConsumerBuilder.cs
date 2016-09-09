using System;
using System.Collections.Generic;
using System.Reflection;
using Meceqs.Pipeline;
using Meceqs.TypedHandling;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Transport
{
    public abstract class TransportConsumerBuilder<TTransportConsumerBuilder, TTransportConsumerOptions>
        : ITransportConsumerBuilder<TTransportConsumerBuilder>
        where TTransportConsumerBuilder : ITransportConsumerBuilder<TTransportConsumerBuilder>
        where TTransportConsumerOptions : TransportConsumerOptions
    {
        private readonly List<Assembly> _deserializationAssemblies = new List<Assembly>();

        private Action<TTransportConsumerOptions> _consumerOptions;

        private string _pipelineName;
        private Action<IPipelineBuilder> _pipeline;

        /// <summary>
        /// Allows derived classes to modify the user-defined pipeline by adding additional filters 
        /// at the beginning.
        /// </summary>
        protected Action<IPipelineBuilder> PipelineStartHook { get; set; }

        /// <summary>
        /// Allows derived classes to modify the user-defined pipeline by adding additional filters 
        /// at the end.
        /// </summary>
        protected Action<IPipelineBuilder> PipelineEndHook { get; set; }

        /// <summary>
        /// This property is only necessary to support the builder pattern with
        /// the generic arguments from this type.
        /// </summary>
        public abstract TTransportConsumerBuilder Instance { get; }

        public IEnumerable<Assembly> GetDeserializationAssemblies() => _deserializationAssemblies;
        public Action<TTransportConsumerOptions> GetConsumerOptions() => _consumerOptions;
        public string GetPipelineName() => _pipelineName;

        public Action<IPipelineBuilder> GetPipeline()
        {
            //var pipeline = PipelineModifier?.Invoke(_pipeline) ?? _pipeline;

            var pipeline = PipelineStartHook + _pipeline + PipelineEndHook;

            if (pipeline == null)
            {
                throw new MeceqsException(
                    $"No pipeline was configured. You can either use TypedHandling by calling " +
                    $"'{nameof(UseTypedHandling)}' or you can configure a custom pipeline " +
                    $"by calling '{nameof(ConfigurePipeline)}'.");
            }

            return pipeline;
        }

        public TTransportConsumerBuilder AddMessageType<TMessage>()
        {
            return AddMessageType(typeof(TMessage));
        }

        public TTransportConsumerBuilder AddMessageType<TMessage, TResult>()
        {
            return AddMessageType(typeof(TMessage), typeof(TResult));
        }

        public TTransportConsumerBuilder AddMessageType(Type messageType, Type resultType = null)
        {
            Check.NotNull(messageType, nameof(messageType));

            _consumerOptions += x => x.AddMessageType(messageType, resultType);

            // To be able to work with the message in the consumer,
            // we must also be able to deserialize it.
            if (!_deserializationAssemblies.Contains(messageType.GetTypeInfo().Assembly))
            {
                _deserializationAssemblies.Add(messageType.GetTypeInfo().Assembly);
            }

            return Instance;
        }

        public TTransportConsumerBuilder SetUnknownMessageBehavior(UnknownMessageBehavior behavior)
        {
            _consumerOptions += x => x.UnknownMessageBehavior = behavior;
            return Instance;
        }

        public TTransportConsumerBuilder ThrowOnUnknownMessage()
        {
            return SetUnknownMessageBehavior(UnknownMessageBehavior.ThrowException);
        }

        public TTransportConsumerBuilder SkipUnknownMessages()
        {
            return SetUnknownMessageBehavior(UnknownMessageBehavior.Skip);
        }

        public TTransportConsumerBuilder ConfigurePipeline(Action<IPipelineBuilder> pipeline)
        {
            return ConfigurePipeline(null, pipeline);
        }

        public TTransportConsumerBuilder ConfigurePipeline(string pipelineName, Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            _pipelineName = pipelineName;
            _pipeline = pipeline;
            return Instance;
        }

        public TTransportConsumerBuilder UseTypedHandling(Action<TypedHandlingOptions> options)
        {
            Check.NotNull(options, nameof(options));

            var handlingOptions = new TypedHandlingOptions();
            options(handlingOptions);

            return UseTypedHandling(handlingOptions);
        }

        public TTransportConsumerBuilder UseTypedHandling(TypedHandlingOptions options)
        {
            Check.NotNull(options, nameof(options));

            // We must also tell the actual transport that it should accept the message types!
            foreach (var handler in options.Handlers)
            {
                foreach (var implementedHandle in handler.ImplementedHandles)
                {
                    AddMessageType(implementedHandle.Item1, implementedHandle.Item2);
                }
            }

            // Add the filter to the end of the pipeline.
            PipelineEndHook = pipeline =>
            {
                pipeline.RunTypedHandling(options);
            };

            return Instance;
        }
    }
}