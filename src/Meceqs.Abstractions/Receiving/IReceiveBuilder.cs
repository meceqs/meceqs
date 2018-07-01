using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Receiving
{
    /// <summary>
    /// Represents a builder object for receiving an existing envelope from an external caller/system.
    /// </summary>
    public interface IReceiveBuilder : IMessageContextBuilder<IReceiveBuilder>
    {
        /// <summary>
        /// Sends the envelope to the pipeline. If no pipeline name was configured,
        /// the default "Receive" pipeline will be used.
        /// </summary>
        Task ReceiveAsync();

        /// <summary>
        /// Sends the envelope to the pipeline and expects a response object of the given type.
        /// If no pipeline name was configured, the default "Receive" pipeline will be used.
        /// </summary>
        Task<TResponse> ReceiveAsync<TResponse>();

        /// <summary>
        /// Sends the envelope to the pipeline and expects a response object of the given type.
        /// If no pipeline name was configured, the default "Receive" pipeline will be used.
        /// </summary>
        Task<object> ReceiveAsync(Type responseType);
    }
}