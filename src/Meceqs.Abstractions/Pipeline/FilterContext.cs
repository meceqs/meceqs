using System;
using System.Security.Claims;
using System.Threading;

namespace Meceqs.Pipeline
{
    public class FilterContext<TMessage> : FilterContext where TMessage : class
    {
        public new Envelope<TMessage> Envelope => (Envelope<TMessage>)base.Envelope;

        public new TMessage Message => (TMessage)base.Message;

        public FilterContext(Envelope<TMessage> envelope)
            : base(envelope)
        {
        }
    }

    /// <summary>
    /// Contains the envelope and metadata about the current execution.
    /// </summary>
    public abstract class FilterContext
    {
        /// <summary>
        /// Gets the envelope for which the pipeline is executed.
        /// <summary>
        public Envelope Envelope { get; }

        /// <summary>
        /// A shortcut for <see cref="Envelope.Message"/>.
        /// <summary>
        public object Message => Envelope.Message; // just for faster access to the message

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        public Type MessageType => Envelope.Message.GetType();

        /// <summary>
        /// Gets the name of the pipeline on which the current filter is executed.
        /// </summary>
        public string PipelineName { get; private set; }

        /// <summary>
        /// Gets the service provider for the current execution.
        /// </summary>
        public IServiceProvider RequestServices { get; private set; }

        /// <summary>
        /// Gets the type of the result expected by the caller.
        /// </summary>
        public Type ExpectedResultType { get; private set; }

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
        /// <see cref="Items"/> can be used to pass data from one filter to another.
        /// </summary>
        public FilterContextItems Items { get; } = new FilterContextItems();

        /// <summary>
        /// Gets or sets the user for the current execution.
        /// </summary>
        public ClaimsPrincipal User { get; set; }

        protected FilterContext(Envelope envelope)
        {
            Check.NotNull(envelope, nameof(envelope));

            Envelope = envelope;
        }

        public void Initialize(string pipelineName, IServiceProvider requestServices, Type expectedResultType)
        {
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));
            Check.NotNull(requestServices, nameof(requestServices));

            PipelineName = pipelineName;
            RequestServices = requestServices;
            ExpectedResultType = expectedResultType;
        }
    }
}