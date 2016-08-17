using System;
using Meceqs;
using Meceqs.Filters.Auditing;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuditingPipelineBuilderExtensions
    {
        public static IPipelineBuilder UseAuditing(this IPipelineBuilder builder, Action<AuditingOptions> setupAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new AuditingOptions();
            setupAction?.Invoke(options);

            builder.UseFilter<AuditingFilter>(options);

            return builder;
        }
    }
}