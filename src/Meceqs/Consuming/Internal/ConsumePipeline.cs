using Meceqs.Pipeline;
using Microsoft.Extensions.Options;

namespace Meceqs.Consuming.Internal
{
    public class ConsumePipeline : IConsumePipeline
    {
        public IPipeline Pipeline { get; }

        public ConsumePipeline(IOptions<ConsumeOptions> options)
        {
            Check.NotNull(options, nameof(options));

            Pipeline = options.Value.Pipeline.GetPipeline();
        }
    }
}