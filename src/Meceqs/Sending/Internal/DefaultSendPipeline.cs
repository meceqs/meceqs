using Meceqs.Pipeline;
using Microsoft.Extensions.Options;

namespace Meceqs.Sending.Internal
{
    public class DefaultSendPipeline : ISendPipeline
    {
        public IPipeline Pipeline { get; }

        public DefaultSendPipeline(IOptions<SendOptions> options)
        {
            Check.NotNull(options, nameof(options));

            Pipeline = new DefaultPipeline(options.Value.Pipeline);
        }
    }
}