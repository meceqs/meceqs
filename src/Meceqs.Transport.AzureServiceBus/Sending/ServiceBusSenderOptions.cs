namespace Meceqs.Transport.AzureServiceBus.Sending
{
    public class ServiceBusSenderOptions
    {
        public string ConnectionString { get; set; }
        public string EntityPath { get; set; }
    }
}