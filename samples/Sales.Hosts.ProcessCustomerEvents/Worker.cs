using Meceqs.Transport.AzureEventHubs.FileMock;

namespace Sales.Hosts.ProcessCustomerEvents
{
    public class Worker
    {
        private readonly FileMockEventHubProcessor _eventProcessor;

        public Worker(FileMockEventHubProcessor eventProcessor)
        {
            _eventProcessor = eventProcessor;
        }

        public void Run()
        {
            _eventProcessor.Start();
        }
    }
}