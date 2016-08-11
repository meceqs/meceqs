using Meceqs;
using Meceqs.Filters.EnvelopeSanitizer;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EnvelopeSanitizerPipelineBuilderExtensions
    {
        public static IPipelineBuilder UseEnvelopeSanitizer(this IPipelineBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.UseFilter<EnvelopeSanitizerFilter>();

            return builder;
        }
    }
}