using Meceqs.Pipeline;
using Microsoft.Extensions.Options;

namespace Meceqs.Sending.Internal
{
    public class SendPipeline : ISendPipeline
    {
        public IPipeline Pipeline { get; }

        public SendPipeline(IOptions<SendOptions> options)
        {
            Check.NotNull(options, nameof(options));

            Pipeline = options.Value.Pipeline.GetPipeline();
        }
    }
}