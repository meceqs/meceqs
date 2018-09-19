using Meceqs.TypedHandling;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Reflection;
using Meceqs.Pipeline;

namespace Meceqs.Transport
{
    public abstract class ReceiveTransportBuilder<TReceiveTransportBuilder, TReceiveTransportOptions> : TransportPipelineBuilder
        where TReceiveTransportBuilder : ReceiveTransportBuilder<TReceiveTransportBuilder, TReceiveTransportOptions>
        where TReceiveTransportOptions : ReceiveTransportOptions
    {
        /// <summary>
        /// This property is only necessary to support the builder pattern with
        /// the generic arguments from this type.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected abstract TReceiveTransportBuilder Instance { get; }

        protected ReceiveTransportBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName ?? MeceqsDefaults.ReceivePipelineName)
        {
            Services.Configure<TReceiveTransportOptions>(PipelineName, PipelineConfiguration);
        }

        public TReceiveTransportBuilder Configure(Action<TReceiveTransportOptions> options)
        {
            if (options != null)
            {
                Services.Configure(PipelineName, options);
            }

            return Instance;
        }

        public TReceiveTransportBuilder ConfigurePipeline(Action<IPipelineBuilder> pipeline)
        {
            ConfigurePipelineInternal(pipeline);
            return Instance;
        }

        public TReceiveTransportBuilder AddMessageType<TMessage>()
        {
            return AddMessageType(typeof(TMessage));
        }

        public TReceiveTransportBuilder AddMessageType<TMessage, TResponse>()
        {
            return AddMessageType(typeof(TMessage), typeof(TResponse));
        }

        public TReceiveTransportBuilder AddMessageType(Type messageType, Type responseType = null)
        {
            Guard.NotNull(messageType, nameof(messageType));

            Configure(x => x.AddMessageType(messageType, responseType));

            // To be able to work with the message in the receiver,
            // we must also be able to deserialize it.
            MeceqsBuilder.AddDeserializationAssembly(messageType.GetTypeInfo().Assembly);

            return Instance;
        }

        public TReceiveTransportBuilder SetUnknownMessageBehavior(UnknownMessageBehavior behavior)
        {
            Configure(x => x.UnknownMessageBehavior = behavior);
            return Instance;
        }

        public TReceiveTransportBuilder ThrowOnUnknownMessage()
        {
            return SetUnknownMessageBehavior(UnknownMessageBehavior.ThrowException);
        }

        public TReceiveTransportBuilder SkipUnknownMessages()
        {
            return SetUnknownMessageBehavior(UnknownMessageBehavior.Skip);
        }

        public TReceiveTransportBuilder UseTypedHandling(Action<TypedHandlingOptions> options)
        {
            Guard.NotNull(options, nameof(options));

            var handlingOptions = new TypedHandlingOptions();
            options(handlingOptions);

            return UseTypedHandling(handlingOptions);
        }

        public TReceiveTransportBuilder UseTypedHandling(TypedHandlingOptions options)
        {
            Guard.NotNull(options, nameof(options));

            // We must also tell the actual transport that it should accept the message types!
            foreach (var handler in options.Handlers)
            {
                foreach (var implementedHandle in handler.ImplementedHandles)
                {
                    AddMessageType(implementedHandle.MessageType, implementedHandle.ResponseType);
                }
            }

            // Add the middleware to the end of the pipeline.
            ConfigurePipeline(pipeline => pipeline.EndsWith(x => x.RunTypedHandling(options)));

            return Instance;
        }
    }
}
