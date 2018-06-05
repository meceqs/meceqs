using System;
using System.ComponentModel;
using Meceqs.Pipeline;
using Meceqs.TypedHandling;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Transport
{
    public interface ITransportReceiverBuilder<TTransportReceiver, TTransportReceiverOptions>
        where TTransportReceiver : ITransportReceiverBuilder<TTransportReceiver, TTransportReceiverOptions>
        where TTransportReceiverOptions : TransportReceiverOptions
    {
        /// <summary>
        /// Gets the name of the pipeline configured by this builder.
        /// </summary>
        string PipelineName { get; }

        /// <summary>
        /// Gets the Meceqs builder.
        /// </summary>
        IMeceqsBuilder MeceqsBuilder { get; }

        /// <summary>
        /// Gets the application service collection.
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// This property is only necessary to support the builder pattern with
        /// the generic arguments from this type.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        TTransportReceiver Instance { get; }

        TTransportReceiver Configure(Action<TTransportReceiverOptions> options);

        TTransportReceiver ConfigurePipeline(Action<PipelineOptions> pipeline);

        TTransportReceiver AddMessageType<TMessage>();

        TTransportReceiver AddMessageType<TMessage, TResult>();

        TTransportReceiver AddMessageType(Type messageType, Type resultType = null);

        TTransportReceiver SetUnknownMessageBehavior(UnknownMessageBehavior behavior);

        TTransportReceiver ThrowOnUnknownMessage();

        TTransportReceiver SkipUnknownMessages();

        TTransportReceiver UseTypedHandling(Action<TypedHandlingOptions> options);

        TTransportReceiver UseTypedHandling(TypedHandlingOptions options);
    }
}