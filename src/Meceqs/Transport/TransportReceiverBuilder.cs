using System;
using System.Collections.Generic;
using System.Reflection;
using Meceqs.Configuration;
using Meceqs.Pipeline;
using Meceqs.TypedHandling;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Transport
{
    public abstract class TransportReceiverBuilder<TTransportReceiverBuilder, TTransportReceiverOptions>
        : ITransportReceiverBuilder<TTransportReceiverBuilder>
        where TTransportReceiverBuilder : ITransportReceiverBuilder<TTransportReceiverBuilder>
        where TTransportReceiverOptions : TransportReceiverOptions
    {
        private readonly List<Assembly> _deserializationAssemblies = new List<Assembly>();

        private Action<TTransportReceiverOptions> _receiverOptions;

        private string _pipelineName = MeceqsDefaults.ReceivePipelineName;
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
        public abstract TTransportReceiverBuilder Instance { get; }

        public IEnumerable<Assembly> GetDeserializationAssemblies() => _deserializationAssemblies;
        public Action<TTransportReceiverOptions> GetReceiverOptions() => _receiverOptions;
        public string GetPipelineName() => _pipelineName;

        public Action<IPipelineBuilder> GetPipeline()
        {
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

        public TTransportReceiverBuilder AddMessageType<TMessage>()
        {
            return AddMessageType(typeof(TMessage));
        }

        public TTransportReceiverBuilder AddMessageType<TMessage, TResult>()
        {
            return AddMessageType(typeof(TMessage), typeof(TResult));
        }

        public TTransportReceiverBuilder AddMessageType(Type messageType, Type resultType = null)
        {
            Check.NotNull(messageType, nameof(messageType));

            _receiverOptions += x => x.AddMessageType(messageType, resultType);

            // To be able to work with the message in the receiver,
            // we must also be able to deserialize it.
            if (!_deserializationAssemblies.Contains(messageType.GetTypeInfo().Assembly))
            {
                _deserializationAssemblies.Add(messageType.GetTypeInfo().Assembly);
            }

            return Instance;
        }

        public TTransportReceiverBuilder SetUnknownMessageBehavior(UnknownMessageBehavior behavior)
        {
            _receiverOptions += x => x.UnknownMessageBehavior = behavior;
            return Instance;
        }

        public TTransportReceiverBuilder ThrowOnUnknownMessage()
        {
            return SetUnknownMessageBehavior(UnknownMessageBehavior.ThrowException);
        }

        public TTransportReceiverBuilder SkipUnknownMessages()
        {
            return SetUnknownMessageBehavior(UnknownMessageBehavior.Skip);
        }

        public TTransportReceiverBuilder SetPipelineName(string pipelineName)
        {
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            _pipelineName = pipelineName;
            return Instance;
        }

        public TTransportReceiverBuilder ConfigurePipeline(Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            _pipeline = pipeline;
            return Instance;
        }

        public TTransportReceiverBuilder UseTypedHandling(Action<TypedHandlingOptions> options)
        {
            Check.NotNull(options, nameof(options));

            var handlingOptions = new TypedHandlingOptions();
            options(handlingOptions);

            return UseTypedHandling(handlingOptions);
        }

        public TTransportReceiverBuilder UseTypedHandling(TypedHandlingOptions options)
        {
            Check.NotNull(options, nameof(options));

            // We must also tell the actual transport that it should accept the message types!
            foreach (var handler in options.Handlers)
            {
                foreach (var implementedHandle in handler.ImplementedHandles)
                {
                    AddMessageType(implementedHandle.MessageType, implementedHandle.ResultType);
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