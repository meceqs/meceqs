using System;
using Meceqs.Pipeline;
using Meceqs.TypedHandling;

namespace Meceqs.Transport
{
    public interface ITransportSenderBuilder<TTransportSender>
        where TTransportSender : ITransportSenderBuilder<TTransportSender>
    {
        TTransportSender Instance { get; }

        TTransportSender ConfigurePipeline(Action<IPipelineBuilder> pipeline);

        TTransportSender ConfigurePipeline(string pipelineName, Action<IPipelineBuilder> pipeline);

        TTransportSender UseTypedHandling(Action<TypedHandlingOptions> options);

        TTransportSender UseTypedHandling(TypedHandlingOptions options);
    }
}