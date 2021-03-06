using System.ComponentModel;
using System.Security.Claims;
using System.Threading;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Contains common logic for building a <see cref="MessageContext"/>.
    /// This type is typically not used directly - use <see cref="Receiving.IReceiveBuilder"/>
    /// or <see cref="Sending.ISendBuilder"/> instead.
    /// </summary>
    public interface IMessageContextBuilder<TBuilder>
        where TBuilder : IMessageContextBuilder<TBuilder>
    {
        /// <summary>
        /// Returns the current instance. This is only required for derived types
        /// and should not be used.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        TBuilder Instance { get; }

        /// <summary>
        /// Writes the given cancellation token to <see cref="MessageContext.Cancellation"/>.
        /// </summary>
        TBuilder SetCancellationToken(CancellationToken cancellation);

        /// <summary>
        /// Writes the given key:value pair to <see cref="MessageContext.Items"/>.
        /// </summary>
        TBuilder SetContextItem(string key, object value);

        /// <summary>
        /// Writes the given header:value pair to the <see cref="Envelope.Headers"/> dictionary.
        /// </summary>
        TBuilder SetHeader(string headerName, object value);

        /// <summary>
        /// Writes the given user to <see cref="MessageContext.User"/>.
        /// </summary>
        TBuilder SetUser(ClaimsPrincipal user);

        /// <summary>
        /// Uses a pipeline with the given name instead of the default pipeline
        /// (which would be "Receive" for <see cref="Receiving.IMessageReceiver"/>
        /// or "Send" for <see cref="Sending.IMessageSender"/>).
        /// </summary>
        TBuilder UsePipeline(string pipelineName);
    }
}
