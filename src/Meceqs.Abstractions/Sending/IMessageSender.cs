using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    /// <summary>
    /// This interface is used for "sending a newly created message" or
    /// for "forwarding an existing envelope" to a pipeline.
    public interface IMessageSender
    {
        // TODO @cweiss remove handling of multiple messages?

        /// <summary>
        /// <para>Creates a builder that sends an existing <see cref="Envelope"/> to a pipeline.
        /// This can be used to forward an envelope from a consumer to a sender.</para>
        /// <para>If you don't specify a pipeline name, the default "Send" pipeline will be used.</para>
        /// </summary>
        IFluentSender ForEnvelope(Envelope envelope);

        /// <summary>
        /// <para>Creates a builder that sends a <paramref name="message"/> to a pipeline.</para>
        /// <para>If you don't specify a pipeline name, the default "Send" pipeline will be used.</para>
        /// </summary>
        IFluentSender ForMessage(object message, Guid? messageId = null);

        /// <summary>
        /// <para>Creates a builder that sends a list of <paramref name="messages"/> to a pipeline.</para>
        /// <para>If you don't specify a pipeline name, the default "Send" pipeline will be used.</para>
        /// </summary>
        IFluentSender ForMessages(IEnumerable<object> messages);

        /// <summary>
        /// Sends the message to the default "Send" pipeline. If you want to use a different pipeline
        /// or change some other behavior, use the builder pattern with <see cref="ForMessage"/>.
        /// </summary>
        Task SendAsync(object message, Guid? messageId = null);

        /// <summary>
        /// Sends the message to the default "Send" pipeline and expects a result object of the given type.
        /// If you want to use a different pipeline or change some other behavior,
        /// use the builder pattern with <see cref="ForMessage"/>.
        /// </summary>
        Task<TResult> SendAsync<TResult>(object message, Guid? messageId = null);

        /// <summary>
        /// Sends the messages to the default "Send" pipeline. If you want to use a different pipeline
        /// or change some other behavior, use the builder pattern with <see cref="ForMessage"/>.
        /// </summary>
        Task SendAsync(IEnumerable<object> messages);

    }
}