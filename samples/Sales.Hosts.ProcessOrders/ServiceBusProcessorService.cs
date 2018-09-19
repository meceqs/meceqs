using System.Threading;
using System.Threading.Tasks;
using Meceqs.AzureServiceBus.FileFake;
using Microsoft.Extensions.Hosting;

namespace Sales.Hosts.ProcessOrders
{
    public class ServiceBusProcessorService : IHostedService
    {
        private readonly FileFakeServiceBusProcessor _processor;

        public ServiceBusProcessorService(FileFakeServiceBusProcessor processor)
        {
            _processor = processor;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _processor.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
