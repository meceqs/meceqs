using Meceqs.AzureServiceBus.FileFake;

namespace Sales.Hosts.ProcessOrders
{
    public class Worker
    {
        private readonly FileFakeServiceBusProcessor _processor;

        public Worker(FileFakeServiceBusProcessor processor)
        {
            _processor = processor;
        }

        public void Run()
        {
            _processor.Start();
        }
    }
}