using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Sending
{
    /// <summary>
    /// Represents a builder object for sending a new message to a pipeline
    /// or for forwarding an existing envelope to a pipeline.
    /// </summary>
    public interface ISendBuilder : IMessageContextBuilder<ISendBuilder>
    {
        /// <summary>
        /// Correlates the envelope/message to be sent with the given envelope by
        /// reusing its "correlation id".
        /// </summary>
        ISendBuilder CorrelateWith(Envelope source);

        /// <summary>
        /// Sends the envelope/message to the pipeline. If no pipeline name was configured,
        /// the default "Send" pipeline will be used.
        /// </summary>
        Task SendAsync();

        /// <summary>
        /// Sends the envelope/message to the pipeline and expects a result object of the given type.
        /// If no pipeline name was configured, the default "Send" pipeline will be used.
        /// </summary>
        Task<TResult> SendAsync<TResult>();

        /// <summary>
        /// Sends the envelope/message to the pipeline and expects a result object of the given type.
        /// If no pipeline name was configured, the default "Send" pipeline will be used.
        /// </summary>
        Task<object> SendAsync(Type resultType);
    }
}