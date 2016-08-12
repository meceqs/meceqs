using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Filters.Auditing
{
    public class AuditingFilter
    {
        public static string NameIdentifierHeaderName = "CreatedBy";

        private readonly FilterDelegate _next;

        public AuditingFilter(FilterDelegate next)
        {
            Check.NotNull(next, nameof(next));

            _next = next;
        }

        public Task Invoke(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            if (context.User != null)
            {
                var currentId = context.Envelope.Headers.Get<string>(NameIdentifierHeaderName);
                if (currentId == null)
                {
                    var nameIdentifierClaim = context.User.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                    if (nameIdentifierClaim != null)
                    {
                        context.Envelope.Headers.Set(NameIdentifierHeaderName, nameIdentifierClaim.Value);
                    }
                }
            }

            return _next(context);
        }
    }
}