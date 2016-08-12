using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Filters.Auditing
{
    public class AuditingFilter
    {
        public static string HeaderNameUserId = "CreatedBy";

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
                var userIdFromEnvelope = context.Envelope.Headers.Get<string>(HeaderNameUserId);
                if (userIdFromEnvelope == null)
                {
                    var userIdFromClaim = context.User.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                    if (userIdFromClaim != null)
                    {
                        context.Envelope.Headers.Set(HeaderNameUserId, userIdFromClaim.Value);
                    }
                }
            }

            return _next(context);
        }
    }
}