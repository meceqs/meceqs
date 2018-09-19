using Meceqs.Transport;

namespace Meceqs.AzureServiceBus.Sending
{
    public class ServiceBusSenderOptions : SendTransportOptions
    {
        public string ConnectionString { get; set; }
    }
}
