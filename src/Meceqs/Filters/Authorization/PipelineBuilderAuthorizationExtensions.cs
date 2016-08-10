using Meceqs.Filters.Authorization;

namespace Meceqs.Pipeline
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