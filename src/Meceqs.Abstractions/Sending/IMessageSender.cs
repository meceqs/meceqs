using System;
using System.Collections.Generic;

namespace Meceqs.Sending
{
    public interface IMessageSender
    {
        // TODO @cweiss remove handling of multiple messages/events?

        /// <summary>
        /// Creates a builder that sends an existing <see cref="Envelope"/> to a pipeline.
        /// This can be used to forward an envelope from a consumer to a sender.
        /// </summary>
        IFluentSender ForEnvelope(Envelope envelope);

        /// <summary>
        /// Creates a builder that sends a list of existing <see cref="Envelope"/>s to a pipeline.
        /// This can be used to forward envelopes from a consumer to a sender.
        /// </summary>
        IFluentSender ForEnvelopes(IEnumerable<Envelope> envelopes);

        /// <summary>
        /// Creates a builder that sends a <paramref name="message"/> to a pipeline.
        /// </summary>
        IFluentSender ForMessage(object message, Guid? messageId = null);

        /// <summary>
        /// Creates a builder that sends a list of <paramref name="messages"/> to a pipeline.
        /// </summary>
        IFluentSender ForMessages<TMessage>(IEnumerable<TMessage> messages) where TMessage : class;
    }
}