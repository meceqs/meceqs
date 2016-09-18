using System;
using Meceqs.Pipeline;

namespace Meceqs.Transport
{
    public interface ITransportSenderBuilder<TTransportSender>
        where TTransportSender : ITransportSenderBuilder<TTransportSender>
    {
        TTransportSender Instance { get; }

        TTransportSender ConfigurePipeline(Action<IPipelineBuilder> pipeline);

        TTransportSender ConfigurePipeline(string pipelineName, Action<IPipelineBuilder> pipeline);
    }
}