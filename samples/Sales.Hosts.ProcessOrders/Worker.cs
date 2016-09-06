using Meceqs.Transport.AzureServiceBus.FileMock;

namespace Sales.Hosts.ProcessOrders
{
    public class Worker
    {
        private readonly FileMockServiceBusProcessor _processor;

        public Worker(FileMockServiceBusProcessor processor)
        {
            _processor = processor;
        }

        public void Run()
        {
            _processor.Start();
        }
    }
}