using Meceqs.Transport;

namespace Meceqs.AzureServiceBus.Sending
{
    public class ServiceBusSenderOptions : TransportSenderOptions
    {
        public string ConnectionString { get; set; }
        public string EntityPath { get; set; }
    }
}