using Meceqs.Channels;

namespace Meceqs.Consuming.Internal
{
    public interface IConsumeChannel
    {
        IChannel Channel { get; }
    }
}