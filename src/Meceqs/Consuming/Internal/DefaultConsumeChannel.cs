using Meceqs.Channels;
using Microsoft.Extensions.Options;

namespace Meceqs.Consuming.Internal
{
    public class DefaultConsumeChannel : IConsumeChannel
    {
        public IChannel Channel { get; }

        public DefaultConsumeChannel(IOptions<ConsumeOptions> options)
        {
            Check.NotNull(options, nameof(options));

            Channel = new DefaultChannel(options.Value.Channel);
        }
    }
}