using System.IO;
using System.Text;
using System.Threading.Tasks;
using Meceqs.Transport.AzureEventHubs;
using Meceqs.Transport.AzureEventHubs.Internal;
using Microsoft.ServiceBus.Messaging;
using SampleConfig;

namespace Customers.Hosts.WebApi.Infrastructure
{
    public class FakeEventHubClientFactory : IEventHubClientFactory
    {
        public IEventHubClient CreateEventHubClient(EventHubConnection connection)
        {
            return FakeEventHubClient.Instance;
        }
    }

    public class FakeEventHubClient : IEventHubClient
    {
        public static readonly IEventHubClient Instance = new FakeEventHubClient();

        public Task SendAsync(EventData data)
        {
            using (StreamReader reader = new StreamReader(data.GetBodyStream(), Encoding.UTF8))
            {
                string json = reader.ReadToEnd();
                File.AppendAllLines(SampleConfiguration.CustomerEventsFile, new [] { json });
            }

            return Task.CompletedTask;
        }

        public void Close()
        {
        }
    }
}