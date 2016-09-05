using Meceqs;
using Meceqs.Filters.MessageIgnorer;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MessageIgnorerPipelineBuilderExtensions
    {
        public static IPipelineBuilder RunMessageIgnorer(this IPipelineBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.UseFilter<MessageIgnorerFilter>();

            return builder;
        }
    }
}