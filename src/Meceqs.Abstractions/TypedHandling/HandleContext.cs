using System;
using System.Reflection;
using Meceqs.Pipeline;
using Meceqs.Sending;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.TypedHandling
{
    public class HandleContext<TMessage> : HandleContext where TMessage : class
    {
        public new FilterContext<TMessage> FilterContext => (FilterContext<TMessage>)base.FilterContext;

        public new Envelope<TMessage> Envelope => FilterContext.Envelope;

        public new TMessage Message => FilterContext.Message;

        public HandleContext(FilterContext<TMessage> filterContext)
            : base(filterContext)
        {
        }
    }

    public abstract class HandleContext
    {
        public FilterContext FilterContext { get; }

        public Envelope Envelope => FilterContext.Envelope;

        public object Message => FilterContext.Message;

        public IHandles Handler { get; set; }

        public Type HandlerType { get; set; }

        public MethodInfo HandleMethod { get; set; }

        public IMessageSender MessageSender => FilterContext.RequestServices.GetRequiredService<IMessageSender>();

        protected HandleContext(FilterContext filterContext)
        {
            Check.NotNull(filterContext, nameof(filterContext));

            FilterContext = filterContext;
        }
    }
}