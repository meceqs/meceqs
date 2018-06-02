using System;
using System.Security.Claims;
using System.Threading;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Contains the envelope and metadata about the current execution.
    /// </summary>
    public class MessageContext
    {
        private MessageContextItems _messageContextItems;

        /// <summary>
        /// Gets the envelope for which the pipeline is executed.
        /// </summary>
        public Envelope Envelope { get; }

        /// <summary>
        /// Gets the message for which the pipeline is executed. This is a shortcut for <see cref="Envelope.Message"/>.
        /// </summary>
        public object Message => Envelope.Message; // just for faster access to the message

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        public Type MessageType => Envelope.Message.GetType();

        /// <summary>
        /// Gets the name of the pipeline on which the current middleware is executed.
        /// </summary>
        public string PipelineName { get; }

        /// <summary>
        /// Gets the service provider for the current execution.
        /// </summary>
        public IServiceProvider RequestServices { get; }

        /// <summary>
        /// Gets the type of the result expected by the caller.
        /// </summary>
        public Type ExpectedResultType { get; }

        /// <summary>
        /// Gets or sets the result of the current execution. This object must match the type of <see cref="ExpectedResultType"/>
        /// or the caller will receive an invalid cast exception.
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token for the current execution.
        /// </summary>
        public CancellationToken Cancellation { get; set; }

        /// <summary>
        /// <see cref="Items"/> can be used to pass data from one middleware to another.
        /// </summary>
        public MessageContextItems Items
        {
            get
            {
                return _messageContextItems ?? (_messageContextItems = new MessageContextItems());
            }
        }

        /// <summary>
        /// Gets or sets the user for the current execution.
        /// </summary>
        public ClaimsPrincipal User { get; set; }

        public MessageContext(Envelope envelope, string pipelineName, IServiceProvider requestServices, Type expectedResultType)
        {
            Guard.NotNull(envelope, nameof(envelope));
            Guard.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));
            Guard.NotNull(requestServices, nameof(requestServices));
            Guard.NotNull(expectedResultType, nameof(expectedResultType));

            Envelope = envelope;
            PipelineName = pipelineName;
            RequestServices = requestServices;
            ExpectedResultType = expectedResultType;
        }
    }
}