using System;

namespace Meceqs
{
    /// <summary>
    /// An envelope that is strongly typed to its containing message.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message contained in the envelope.</typeparam>
    public class Envelope<TMessage> : Envelope where TMessage : class
    {
        /// <summary>
        /// The strongly-typed message.
        /// </summary>
        public new TMessage Message
        {
            get { return (TMessage)base.Message; }
            set { base.Message = value; }
        }

        public Envelope()
        {
        }

        public Envelope(TMessage message, Guid messageId)
            : base(message, messageId)
        {
        }
    }

    /// <summary>
    /// The untyped base class for an envelope, containing an arbitrary message.
    /// </summary>
    /// <remarks>
    /// If we would only have the strongly-typed version, every class would need
    /// the generic arguments as well, even though most of them don't need to know
    /// about the actual message type at compile time.
    /// </remarks>
    public abstract class Envelope
    {
        /// <summary>
        /// An arbitrary message.
        /// </summary>
        public object Message { get; set; }

        /// <summary>
        /// The unique id of the message.
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// A unique identifier of the message type, used for deserialization.
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// An id that is shared amongst a series of connected messages.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// The creation date of the message.
        /// </summary>
        public DateTime? CreatedOnUtc { get; set; } // TODO @cweiss Use DateTimeOffset?

        /// <summary>
        /// Contains additional, non-typed information about the message.
        /// </summary>
        public EnvelopeProperties Headers { get; set; } = new EnvelopeProperties();

        protected Envelope()
        {
        }

        protected Envelope(object message, Guid messageId)
        {
            Guard.NotNull(message, nameof(message));
            Guard.NotEmpty(messageId, nameof(messageId));

            Message = message;
            MessageId = messageId;

            Sanitize();
        }

        /// <summary>
        /// Validates the absolute minimum requirements for an envelope.
        /// If they are not satisfied, processing the envelope is not useful.
        /// </summary>
        public void EnsureValid()
        {
            Guard.NotNull(Message, nameof(Message));
            Guard.NotEmpty(MessageId, nameof(MessageId));
            Guard.NotNullOrWhiteSpace(MessageType, nameof(MessageType));
        }

        /// <summary>
        /// Sets default values for certain envelope properties in case they are not present.
        /// </summary>
        public void Sanitize()
        {
            if (Message != null)
            {
                Type messageType = Message.GetType();

                // if the envelope is not deserialized in a statically typed way (e.g. through ASP.NET MVC ModelBinding),
                // the value for MessageType could be wrong or missing. (e.g. because the request was made with Fiddler)
                // For this reason, we just re-set it!
                MessageType = messageType.FullName;
            }

            if (MessageId == Guid.Empty)
            {
                MessageId = Guid.NewGuid();
            }

            if (CorrelationId == Guid.Empty)
            {
                // Reusing the messageId from the first message in a series makes it easier
                // to identify the initiating message.
                //
                // This id will be overwritten, when the message is correlated with another message.
                CorrelationId = MessageId;
            }

            if (!CreatedOnUtc.HasValue)
            {
                CreatedOnUtc = DateTime.UtcNow;
            }
        }
    }
}