using Meceqs.Pipeline;
using Microsoft.Extensions.Options;

namespace Meceqs.Sending.Internal
{
    public class SendOptionsSetup : IConfigureOptions<SendOptions>
    {
        private readonly IPipelineBuilder _pipelineBuilder;

        public SendOptionsSetup(IPipelineBuilder pipelineBuilder)
        {
            _pipelineBuilder = pipelineBuilder;
        }

        public void Configure(SendOptions options)
        {
            options.Channel.PipelineBuilder = _pipelineBuilder;
        }
    }
}