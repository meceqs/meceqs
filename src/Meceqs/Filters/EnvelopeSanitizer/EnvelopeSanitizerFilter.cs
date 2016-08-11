using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Filters.EnvelopeSanitizer
{
    public class EnvelopeSanitizerFilter
    {
        private readonly FilterDelegate _next;

        public EnvelopeSanitizerFilter(FilterDelegate next)
        {
            Check.NotNull(next, nameof(next));

            _next = next;
        }

        public Task Invoke(FilterContext context)
        {
            // TODO @cweiss is this a good solution?

            Type messageType = context.Message.GetType();

            // if the envelope is not deserialized in a statically typed way (e.g. through ASP.NET MVC ModelBinding),
            // the values for MessageName and -Type could be wrong or missing. (e.g. because the request was made with Fiddler)
            // For this reason, we just re-set them!
            context.Envelope.MessageName = messageType.Name;
            context.Envelope.MessageType = messageType.FullName;

            if (context.Envelope.MessageId == Guid.Empty)
            {
                context.Envelope.MessageId = Guid.NewGuid();
            }

            if (!context.Envelope.CorrelationId.HasValue || context.Envelope.CorrelationId.Value == Guid.Empty)
            {
                context.Envelope.CorrelationId = Guid.NewGuid();
            }

            if (!context.Envelope.CreatedOnUtc.HasValue)
            {
                context.Envelope.CreatedOnUtc = DateTime.UtcNow;
            }

            return _next(context);
        }
    }
}