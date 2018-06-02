using System;
using Meceqs.Pipeline;

namespace Meceqs.Transport
{
    public interface ITransportSenderBuilder<TTransportSender>
        where TTransportSender : ITransportSenderBuilder<TTransportSender>
    {
        TTransportSender Instance { get; }

        TTransportSender SetPipelineName(string pipelineName);

        TTransportSender ConfigurePipeline(Action<PipelineOptions> pipeline);
    }
}