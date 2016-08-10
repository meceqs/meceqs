using Meceqs.Channels;

namespace Meceqs.Sending.Internal
{
    public interface ISendChannel
    {
        IChannel Channel { get; }
    }
}