using System;
using Meceqs;
using Meceqs.Middleware.Auditing;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuditingPipelineBuilderExtensions
    {
        public static IPipelineBuilder UseAuditing(this IPipelineBuilder pipeline, Action<AuditingOptions> setupAction = null)
        {
            Guard.NotNull(pipeline, nameof(pipeline));

            var options = new AuditingOptions();
            setupAction?.Invoke(options);

            pipeline.UseMiddleware<AuditingMiddleware>(options);

            return pipeline;
        }
    }
}
