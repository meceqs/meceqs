using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Consuming
{
    /// <summary>
    /// Represents a builder object for consuming an existing envelope from an external caller/system.
    /// </summary>
    public interface IConsumeBuilder : IFilterContextBuilder<IConsumeBuilder>
    {
        /// <summary>
        /// Sends the envelope to the pipeline. If no pipeline name was configured,
        /// the default "Consume" pipeline will be used.
        /// </summary>
        Task ConsumeAsync();

        /// <summary>
        /// Sends the envelope to the pipeline and expects a result object of the given type.
        /// If no pipeline name was configured, the default "Consume" pipeline will be used.
        /// </summary>
        Task<TResult> ConsumeAsync<TResult>();

        /// <summary>
        /// Sends the envelope to the pipeline and expects a result object of the given type.
        /// If no pipeline name was configured, the default "Consume" pipeline will be used.
        /// </summary>
        Task<object> ConsumeAsync(Type resultType);
    }
}