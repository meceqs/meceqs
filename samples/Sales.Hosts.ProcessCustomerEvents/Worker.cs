using Meceqs.AzureEventHubs.FileFake;

namespace Sales.Hosts.ProcessCustomerEvents
{
    public class Worker
    {
        private readonly FileFakeEventHubProcessor _eventProcessor;

        public Worker(FileFakeEventHubProcessor eventProcessor)
        {
            _eventProcessor = eventProcessor;
        }

        public void Run()
        {
            _eventProcessor.Start();
        }
    }
}