using System;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using Meceqs.Pipeline;
using Meceqs.Sending;

namespace Meceqs.TypedHandling
{
    /// <summary>
    /// Contains the envelope and metadata about the current execution.
    /// Use <see cref="MessageContext"/> to access the
    /// <see cref="Pipeline.MessageContext"/> of the current execution.
    /// </summary>
    public class HandleContext
    {
        private IMessageSender _messageSender;

        /// <summary>
        /// Gets the message context of the current execution.
        /// </summary>
        public MessageContext MessageContext { get; }

        /// <summary>
        /// Gets the handler which processes the envelope/message.
        /// </summary>
        public IHandles Handler { get; }

        /// <summary>
        /// Gets the type of the handler which processes the envelope/message.
        /// This can be used to read custom attributes of that type - e.g. by an interceptor.
        /// </summary>
        public Type HandlerType => Handler.GetType();

        /// <summary>
        /// Gets the "HandleAsync" method which processes the envelope/message.
        /// This can be used to read custom attributes of that method - e.g. by an interceptor.
        /// </summary>
        public MethodInfo HandleMethod { get; }

        #region MessageContext Shortcuts

        /// <summary>
        /// Gets the envelope for which the pipeline is executed.
        /// This is a shortcut for <see cref="MessageContext.Envelope"/>.
        /// </summary>
        public Envelope Envelope => MessageContext.Envelope;

        /// <summary>
        /// Gets the message for which the pipeline is executed.
        /// This is a shortcut for <see cref="MessageContext.Message"/>.
        /// </summary>
        public object Message => MessageContext.Message;

        /// <summary>
        /// Gets the type of the message.
        /// This is a shortcut for <see cref="MessageContext.MessageType"/>.
        /// </summary>
        public Type MessageType => MessageContext.MessageType;

        /// <summary>
        /// Gets the name of the pipeline on which the current middleware is executed.
        /// This is a shortcut for <see cref="MessageContext.PipelineName"/>.
        /// </summary>
        public string PipelineName => MessageContext.PipelineName;

        /// <summary>
        /// Gets the service provider for the current execution.
        /// This is a shortcut for <see cref="MessageContext.RequestServices"/>.
        /// </summary>
        public IServiceProvider RequestServices => MessageContext.RequestServices;

        /// <summary>
        /// Gets the cancellation token for the current execution.
        /// This is a shortcut for <see cref="MessageContext.Cancellation"/>.
        /// </summary>
        public CancellationToken Cancellation => MessageContext.Cancellation;

        /// <summary>
        /// <see cref="Items"/> can be used to pass data from one middleware to another.
        /// This is a shortcut for <see cref="MessageContext.Items"/>.
        /// </summary>
        public MessageContextItems Items => MessageContext.Items;

        /// <summary>
        /// Gets the user for the current execution.
        /// This is a shortcut for <see cref="MessageContext.User"/>.
        /// </summary>
        public ClaimsPrincipal User => MessageContext.User;

        #endregion

        /// <summary>
        /// Gets a <see cref="IMessageSender"/>, resolved from <see cref="MessageContext.RequestServices"/>.
        /// </summary>
        public IMessageSender MessageSender
        {
            get
            {
                if (_messageSender == null)
                {
                    _messageSender = (IMessageSender)MessageContext.RequestServices.GetService(typeof(IMessageSender));
                }
                return _messageSender;
            }
        }

        public HandleContext(MessageContext messageContext, IHandles handler, MethodInfo handleMethod)
        {
            Guard.NotNull(messageContext, nameof(messageContext));
            Guard.NotNull(handler, nameof(handler));
            Guard.NotNull(handleMethod, nameof(handleMethod));

            MessageContext = messageContext;
            Handler = handler;
            HandleMethod = handleMethod;
        }
    }
}