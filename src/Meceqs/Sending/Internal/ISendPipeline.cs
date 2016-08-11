using Meceqs.Pipeline;

namespace Meceqs.Sending.Internal
{
    public interface ISendPipeline
    {
        IPipeline Pipeline { get; }
    }
}