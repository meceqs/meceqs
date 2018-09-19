using Microsoft.Azure.ServiceBus;

namespace Meceqs.AzureServiceBus.Internal
{
    public interface IServiceBusMessageConverter
    {
        Message ConvertToServiceBusMessage(Envelope envelope);

        Envelope ConvertToEnvelope(Message serviceBusMessage);
    }
}
