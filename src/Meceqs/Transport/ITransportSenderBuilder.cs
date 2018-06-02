using System;
using System.ComponentModel;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Transport
{
    public interface ITransportSenderBuilder<TTransportSender, TTransportSenderOptions>
        where TTransportSender : ITransportSenderBuilder<TTransportSender, TTransportSenderOptions>
        where TTransportSenderOptions : TransportSenderOptions
    {
        /// <summary>
        /// Gets the name of the pipeline configured by this builder.
        /// </summary>
        string PipelineName { get; }

        /// <summary>
        /// Gets the application service collection.
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// This property is only necessary to support the builder pattern with
        /// the generic arguments from this type.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        TTransportSender Instance { get; }

        TTransportSender Configure(Action<TTransportSenderOptions> options);

        TTransportSender ConfigurePipeline(Action<PipelineOptions> pipeline);
    }
}