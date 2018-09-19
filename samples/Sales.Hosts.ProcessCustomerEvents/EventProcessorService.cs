using System.Threading;
using System.Threading.Tasks;
using Meceqs.AzureEventHubs.FileFake;
using Microsoft.Extensions.Hosting;

namespace Sales.Hosts.ProcessCustomerEvents
{
    public class EventProcessorService : IHostedService
    {
        private readonly FileFakeEventHubProcessor _eventProcessor;

        public EventProcessorService(FileFakeEventHubProcessor eventProcessor)
        {
            _eventProcessor = eventProcessor;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _eventProcessor.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
