using Meceqs.Pipeline;
using Microsoft.Extensions.Options;

namespace Meceqs.Consuming.Internal
{
    public class DefaultConsumePipeline : IConsumePipeline
    {
        public IPipeline Pipeline { get; }

        public DefaultConsumePipeline(IOptions<ConsumeOptions> options)
        {
            Check.NotNull(options, nameof(options));

            Pipeline = new DefaultPipeline(options.Value.Pipeline);
        }
    }
}