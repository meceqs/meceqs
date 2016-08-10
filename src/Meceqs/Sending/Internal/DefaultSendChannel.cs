using Meceqs.Channels;
using Microsoft.Extensions.Options;

namespace Meceqs.Sending.Internal
{
    public class DefaultSendChannel : ISendChannel
    {
        public IChannel Channel { get; }

        public DefaultSendChannel(IOptions<SendOptions> options)
        {
            Check.NotNull(options, nameof(options));

            Channel = new DefaultChannel(options.Value.Channel);
        }
    }
}