using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Receiving
{
    /// <summary>
    /// Represents a builder object for receiving an existing envelope from an external caller/system.
    /// </summary>
    public interface IReceiveBuilder : IFilterContextBuilder<IReceiveBuilder>
    {
        /// <summary>
        /// Sends the envelope to the pipeline. If no pipeline name was configured,
        /// the default "Receive" pipeline will be used.
        /// </summary>
        Task ReceiveAsync();

        /// <summary>
        /// Sends the envelope to the pipeline and expects a result object of the given type.
        /// If no pipeline name was configured, the default "Receive" pipeline will be used.
        /// </summary>
        Task<TResult> ReceiveAsync<TResult>();

        /// <summary>
        /// Sends the envelope to the pipeline and expects a result object of the given type.
        /// If no pipeline name was configured, the default "Receive" pipeline will be used.
        /// </summary>
        Task<object> ReceiveAsync(Type resultType);
    }
}