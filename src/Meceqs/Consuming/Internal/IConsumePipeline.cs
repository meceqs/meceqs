using Meceqs.Pipeline;

namespace Meceqs.Consuming.Internal
{
    public interface IConsumePipeline
    {
        IPipeline Pipeline { get; }
    }
}