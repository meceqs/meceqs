using System;
using Meceqs;
using Meceqs.Middleware.Auditing;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuditingPipelineOptionsExtensions
    {
        public static PipelineBuilder UseAuditing(this PipelineBuilder pipeline, Action<AuditingOptions> setupAction = null)
        {
            Guard.NotNull(pipeline, nameof(pipeline));

            var options = new AuditingOptions();
            setupAction?.Invoke(options);

            pipeline.UseMiddleware<AuditingMiddleware>(options);

            return pipeline;
        }
    }
}