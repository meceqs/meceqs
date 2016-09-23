using System;
using System.ComponentModel;
using System.Security.Claims;
using System.Threading;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Contains common logic for building a <see cref="FilterContext"/>.
    /// This type is typically not used directly - use <see cref="Meceqs.Consuming.IConsumeBuilder"/>
    /// or <see cref="Meceqs.Sending.ISendBuilder"/> instead.
    /// </summary>
    public interface IFilterContextBuilder<TBuilder>
        where TBuilder : IFilterContextBuilder<TBuilder>
    {
        /// <summary>
        /// Returns the current instance. This is only required for derived types
        /// and should not be used.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        TBuilder Instance { get; }

        /// <summary>
        /// Writes the given cancellation token to <see cref="FilterContext.Cancellation"/>.
        /// </summary>
        TBuilder SetCancellationToken(CancellationToken cancellation);

        /// <summary>
        /// Writes the given key:value pair to <see cref="FilterContext.Items"/>.
        /// </summary>
        TBuilder SetContextItem(string key, object value);

        /// <summary>
        /// Writes the given header:value pair to the <see cref="Envelope.Headers"/> dictionary.
        /// </summary>
        TBuilder SetHeader(string headerName, object value);

        /// <summary>
        /// Uses the given service provider for <see cref="FilterContext.RequestServices"/>
        /// instead of the default (which is the same provider either <see cref="Meceqs.Consuming.IMessageConsumer"/>
        /// or <see cref="Meceqs.Sending.IMessageSender"/> was resolved from).
        /// </summary>
        TBuilder SetRequestServices(IServiceProvider requestServices);

        /// <summary>
        /// Writes the given user to <see cref="FilterContext.User"/>.
        /// </summary>
        TBuilder SetUser(ClaimsPrincipal user);

        /// <summary>
        /// Uses a pipeline with the given name instead of the default pipeline
        /// (which would be "Consume" for <see cref="Meceqs.Consuming.IMessageConsumer"/>
        /// or "Send" for <see cref="Meceqs.Sending.IMessageSender"/>).
        TBuilder UsePipeline(string pipelineName);
    }
}