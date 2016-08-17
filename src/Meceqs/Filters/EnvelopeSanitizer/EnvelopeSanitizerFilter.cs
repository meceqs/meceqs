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
            Check.NotNull(context, nameof(context));

            context.Envelope.Sanitize();
            context.Envelope.EnsureValid();

            return _next(context);
        }
    }
}