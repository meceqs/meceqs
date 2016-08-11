using Meceqs;
using Meceqs.Filters.Authorization;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PipelineBuilderAuthorizationExtensions
    {
        public static IPipelineBuilder UseAuthorization(this IPipelineBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            return builder.UseFilter<AuthorizationFilter>();
        }
    }
}