using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Filters.Auditing
{
    public class AuditingFilter
    {
        private readonly FilterDelegate _next;
        private readonly AuditingOptions _options;

        public AuditingFilter(FilterDelegate next, AuditingOptions options)
        {
            Check.NotNull(next, nameof(next));
            Check.NotNull(options, nameof(options));

            _next = next;
            _options = options;
        }

        public Task Invoke(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            if (context.User != null)
            {
                var userIdFromEnvelope = context.Envelope.Headers.Get<string>(_options.UserIdMessageHeaderName);
                if (userIdFromEnvelope == null)
                {
                    var userIdFromClaim = GetValueFromClaim(context.User, _options.UserIdClaimTypes);
                    if (userIdFromClaim != null)
                    {
                        context.Envelope.Headers.Set(_options.UserIdMessageHeaderName, userIdFromClaim);
                    }
                }
            }

            return _next(context);
        }

        private string GetValueFromClaim(ClaimsPrincipal user, IEnumerable<string> claimTypes)
        {
            foreach (var claimType in claimTypes)
            {
                var claim = user.Claims?.FirstOrDefault(x => x.Type == claimType);
                if (claim?.Value != null)
                    return claim.Value;
            }

            return null;
        }
    }
}