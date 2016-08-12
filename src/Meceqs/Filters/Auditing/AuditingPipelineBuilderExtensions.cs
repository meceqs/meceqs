using Meceqs;
using Meceqs.Filters.Auditing;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuditingPipelineBuilderExtensions
    {
        public static IPipelineBuilder UseAuditing(this IPipelineBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.UseFilter<AuditingFilter>();

            return builder;
        }
    }
}