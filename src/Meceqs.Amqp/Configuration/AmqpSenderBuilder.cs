using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Amqp.Configuration
{
    public class AmqpSenderBuilder : TransportSenderBuilder<IAmqpSenderBuilder, AmqpSenderOptions>, IAmqpSenderBuilder
    {
        public override IAmqpSenderBuilder Instance => this;

        public AmqpSenderBuilder()
        {
            PipelineEndHook = pipeline => pipeline.RunAmqpSender();
        }
    }
}