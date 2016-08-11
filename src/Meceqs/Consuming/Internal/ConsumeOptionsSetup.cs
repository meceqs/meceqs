using Meceqs.Pipeline;
using Microsoft.Extensions.Options;

namespace Meceqs.Consuming.Internal
{
    public class ConsumeOptionsSetup : IConfigureOptions<ConsumeOptions>
    {
        private readonly IPipelineBuilder _pipelineBuilder;

        public ConsumeOptionsSetup(IPipelineBuilder pipelineBuilder)
        {
            _pipelineBuilder = pipelineBuilder;
        }

        public void Configure(ConsumeOptions options)
        {
            options.Pipeline.Builder = _pipelineBuilder;
            options.Pipeline.Name = "Consume";
        }
    }
}