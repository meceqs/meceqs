using Meceqs;
using Meceqs.Amqp.Sending;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AmqpSenderPipelineBuilderExtensions
    {
        public static void RunAmqpSender(this IPipelineBuilder pipeline)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            pipeline.UseFilter<AmqpSenderFilter>();
        }
    }
}