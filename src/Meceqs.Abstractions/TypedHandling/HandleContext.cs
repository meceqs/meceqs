using System;
using System.ComponentModel;
using System.Reflection;
using Meceqs.Pipeline;
using Meceqs.Sending;

namespace Meceqs.TypedHandling
{
    /// <summary>
    /// Contains the typed envelope and metadata about the current execution.
    /// Use <see cref="HandleContext.MessageContext"/> to access the
    /// <see cref="MessageContext"/> of the current execution.
    /// </summary>
    public class HandleContext<TMessage> : HandleContext where TMessage : class
    {
        /// <summary>
        /// Gets the message context of the current execution.
        /// </summary>
        public new MessageContext<TMessage> MessageContext => (MessageContext<TMessage>)base.MessageContext;

        /// <summary>
        /// Gets the typed envelope for which the pipeline is executed.
        /// This is a shortcut for <see cref="MessageContext.Envelope"/>.
        /// <summary>
        public new Envelope<TMessage> Envelope => MessageContext.Envelope;

        /// <summary>
        /// Gets the typed message for which the pipeline is executed.
        /// This is a shortcut for <see cref="MessageContext.Envelope.Message"/>.
        /// <summary>
        public new TMessage Message => MessageContext.Message;

        public HandleContext(MessageContext<TMessage> messageContext)
            : base(messageContext)
        {
        }
    }

    /// <summary>
    /// Contains the envelope and metadata about the current execution.
    /// Use <see cref="HandleContext.MessageContext"/> to access the
    /// <see cref="MessageContext"/> of the current execution.
    /// </summary>
    public abstract class HandleContext
    {
        /// <summary>
        /// Gets the message context of the current execution.
        /// </summary>
        public MessageContext MessageContext { get; }

        /// <summary>
        /// Gets the envelope for which the pipeline is executed.
        /// This is a shortcut for <see cref="MessageContext.Envelope"/>.
        /// <summary>
        public Envelope Envelope => MessageContext.Envelope;

        /// <summary>
        /// Gets the message for which the pipeline is executed.
        /// This is a shortcut for <see cref="MessageContext.Envelope.Message"/>.
        /// <summary>
        public object Message => MessageContext.Message;

        /// <summary>
        /// Gets the handler which processes the envelope/message.
        /// </summary>
        public IHandles Handler { get; private set; }

        /// <summary>
        /// Gets the type of the handler which processes the envelope/message.
        /// This can be used to read custom attributes of that type - e.g. by an interceptor.
        /// </summary>
        public Type HandlerType { get; private set; }

        /// <summary>
        /// Gets the "HandleAsync" method which processes the envelope/message.
        /// This can be used to read custom attributes of that method - e.g. by an interceptor.
        public MethodInfo HandleMethod { get; private set; }

        /// <summary>
        /// Gets a <see cref="IMessageSender"/>, resolved from <see cref="MessageContext.RequestServices"/>.
        /// </summary>
        public IMessageSender MessageSender => (IMessageSender)MessageContext.RequestServices.GetService(typeof(IMessageSender));

        protected HandleContext(MessageContext messageContext)
        {
            Check.NotNull(messageContext, nameof(messageContext));

            MessageContext = messageContext;
        }

        /// <summary>
        /// Brings the context into a valid state before the first interceptor/handler is invoked.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Initialize(IHandles handler, MethodInfo handleMethod)
        {
            Check.NotNull(handler, nameof(handler));
            Check.NotNull(handleMethod, nameof(handleMethod));

            Handler = handler;
            HandlerType = handler.GetType();
            HandleMethod = handleMethod;
        }
    }
}