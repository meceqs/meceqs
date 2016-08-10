using Meceqs;
using Meceqs.Filters.EnvelopeSanitizer;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SanitizeEnvelopePipelineBuilderExtensions
    {
        public static IPipelineBuilder UseEnvelopeSanitizer(this IPipelineBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.UseFilter<EnvelopeSanitizerFilter>();

            return builder;
        }
    }
}