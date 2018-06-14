using System;
using System.Reflection;
using Meceqs.Pipeline;
using Meceqs.TypedHandling;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Transport
{
    public abstract class TransportReceiverBuilder<TTransportReceiverBuilder, TTransportReceiverOptions>
        : ITransportReceiverBuilder<TTransportReceiverBuilder, TTransportReceiverOptions>
        where TTransportReceiverBuilder : ITransportReceiverBuilder<TTransportReceiverBuilder, TTransportReceiverOptions>
        where TTransportReceiverOptions : TransportReceiverOptions
    {
        public string PipelineName { get; }

        public IMeceqsBuilder MeceqsBuilder { get; }

        public IServiceCollection Services => MeceqsBuilder.Services;

        /// <inheritdoc/>
        public abstract TTransportReceiverBuilder Instance { get; }

        protected TransportReceiverBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
        {
            Guard.NotNull(meceqsBuilder, nameof(meceqsBuilder));

            MeceqsBuilder = meceqsBuilder;
            PipelineName = pipelineName ?? MeceqsDefaults.ReceivePipelineName;
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
            Guard.NotNull(messageType, nameof(messageType));

            Configure(x => x.AddMessageType(messageType, resultType));

            // To be able to work with the message in the receiver,
            // we must also be able to deserialize it.
            MeceqsBuilder.AddDeserializationAssembly(messageType.GetTypeInfo().Assembly);

            return Instance;
        }

        public TTransportReceiverBuilder Configure(Action<TTransportReceiverOptions> options)
        {
            if (options != null)
            {
                Services.Configure(PipelineName, options);
            }

            return Instance;
        }

        public TTransportReceiverBuilder ConfigurePipeline(Action<PipelineOptions> pipeline)
        {
            if (pipeline != null)
            {
                Services.Configure(PipelineName, pipeline);
            }
            return Instance;
        }
        
        public TTransportReceiverBuilder SetUnknownMessageBehavior(UnknownMessageBehavior behavior)
        {
            Configure(x => x.UnknownMessageBehavior = behavior);
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

        public TTransportReceiverBuilder UseTypedHandling(Action<TypedHandlingOptions> options)
        {
            Guard.NotNull(options, nameof(options));

            var handlingOptions = new TypedHandlingOptions();
            options(handlingOptions);

            return UseTypedHandling(handlingOptions);
        }

        public TTransportReceiverBuilder UseTypedHandling(TypedHandlingOptions options)
        {
            Guard.NotNull(options, nameof(options));

            // We must also tell the actual transport that it should accept the message types!
            foreach (var handler in options.Handlers)
            {
                foreach (var implementedHandle in handler.ImplementedHandles)
                {
                    AddMessageType(implementedHandle.MessageType, implementedHandle.ResultType);
                }
            }

            // Add the middleware to the end of the pipeline.
            ConfigurePipeline(pipeline => pipeline.EndsWith(x => x.RunTypedHandling(options)));

            return Instance;
        }
    }
}