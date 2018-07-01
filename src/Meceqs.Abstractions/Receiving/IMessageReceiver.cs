using System;
using System.Threading.Tasks;

namespace Meceqs.Receiving
{
    /// <summary>
    /// This interface is used for "receiving an existing envelope from an external caller/system" and forwarding it to a pipeline.
    /// </summary>
    public interface IMessageReceiver
    {
        /// <summary>
        /// <para>Returns a builder object that allows to change the behavior of the receive operation.</para>
        /// <para>If you don't specify a pipeline name, the default "Receive" pipeline will be used.</para>
        /// </summary>
        IReceiveBuilder ForEnvelope(Envelope envelope);

        /// <summary>
        /// Sends the envelope to the default "Receive" pipeline. If you want to use a different pipeline
        /// or change some other behavior, use the builder pattern with <see cref="ForEnvelope"/>.
        /// </summary>
        Task ReceiveAsync(Envelope envelope);

        /// <summary>
        /// Sends the envelope to the default "Receive" pipeline and expects a response object of the given type.
        /// If you want to use a different pipeline or change some other behavior,
        /// use the builder pattern with <see cref="ForEnvelope"/>.
        /// </summary>
        Task<TResponse> ReceiveAsync<TResponse>(Envelope envelope);

        /// <summary>
        /// Sends the envelope to the default "Receive" pipeline and expects a response object of the given type.
        /// If you want to use a different pipeline or change some other behavior,
        /// use the builder pattern with <see cref="ForEnvelope"/>.
        /// </summary>
        Task<object> ReceiveAsync(Envelope envelope, Type responseType);
    }
}