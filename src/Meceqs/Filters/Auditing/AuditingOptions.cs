using System.Collections.Generic;
using System.Security.Claims;

namespace Meceqs.Filters.Auditing
{
    public class AuditingOptions
    {
        public string UserIdMessageHeaderName { get; set; } = "CreatedBy";

        public IEnumerable<string> UserIdClaimTypes { get; set; } = new List<string> { ClaimTypes.NameIdentifier };
    }
}