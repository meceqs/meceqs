using System;
using Meceqs;
using Meceqs.Middleware.Auditing;
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

            builder.UseMiddleware<AuditingMiddleware>(options);

            return builder;
        }
    }
}