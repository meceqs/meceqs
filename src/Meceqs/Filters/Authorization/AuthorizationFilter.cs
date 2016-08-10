using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Filters.Authorization
{
    public class AuthorizationFilter
    {
        private readonly FilterDelegate _next;
        private readonly IAuthorizationProvider _authorizationProvider;

        public AuthorizationFilter(FilterDelegate next, IAuthorizationProvider authorizationProvider)
        {
            Check.NotNull(next, nameof(next));
            Check.NotNull(authorizationProvider, nameof(authorizationProvider));

            _next = next;
            _authorizationProvider = authorizationProvider;
        }

        public async Task Invoke(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            Type messageType = context.Message.GetType();

            await _authorizationProvider.AuthorizeAsync(messageType);

            await _next(context);
        }
    }
}