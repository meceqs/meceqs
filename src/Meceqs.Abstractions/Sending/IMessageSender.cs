using System;
using System.Collections.Generic;

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
        IFluentSender ForMessages<TMessage>(IEnumerable<TMessage> messages) where TMessage : class;
    }
}