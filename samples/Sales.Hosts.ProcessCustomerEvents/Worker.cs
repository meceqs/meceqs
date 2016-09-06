using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Transport;
using Meceqs.Transport.AzureEventHubs.Consuming;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json.Linq;
using SampleConfig;

namespace Sales.Hosts.ProcessCustomerEvents
{
    public class Worker
    {
        private static readonly Random _random = new Random();

        private readonly IServiceProvider _applicationServices;
        private readonly ILogger _logger;

        private Timer _createCustomerTimer;

        private int _lineNumber = 0;

        public Worker(IServiceProvider applicationServices, ILoggerFactory loggerFactory)
        {
            _applicationServices = applicationServices;
            _logger = loggerFactory.CreateLogger<Worker>();
        }

        public void Run()
        {
            _logger.LogInformation("Starting timers");

            _createCustomerTimer = new Timer(_ =>
            {
                InvokeTimerMethod("ProcessEvents", ReadEvents, _createCustomerTimer, 1 * 3000);
            }, null, _random.Next(1000) /* first start */, Timeout.Infinite);
        }

        private async Task ReadEvents()
        {
            // no file, no events.
            if (!File.Exists(SampleConfiguration.CustomerEventsFile))
                return;

            // Read new events
            var events = File.ReadLines(SampleConfiguration.CustomerEventsFile).Skip(_lineNumber);
            var count = events.Count();

            _logger.LogInformation("Processing {Count} events", count);

            if (count > 0)
            {
                // process events (if any)
                foreach (var evt in events)
                {
                    await ProcessEvent(evt);

                    // "commit" by moving cursor forward.
                    _lineNumber++;
                }

                _logger.LogInformation("Processing finished");
            }
        }

        private void InvokeTimerMethod(string name, Func<Task> action, Timer timer, int interval)
        {
            try
            {
                _logger.LogInformation("Executing timer method {Method}", name);

                action().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Exception on {Method}", name);
            }
            finally
            {
                // restart timer
                timer.Change(interval, Timeout.Infinite);
            }
        }

        private async Task ProcessEvent(string serializedEvent)
        {
            var serviceScopeFactory = _applicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var requestServices = scope.ServiceProvider;
                var eventHubConsumer = requestServices.GetRequiredService<IEventHubConsumer>();

                // fake "EventData"-Object
                var eventData = new EventData(Encoding.UTF8.GetBytes(serializedEvent));
                JObject jsonEvent = JObject.Parse(serializedEvent);
                eventData.Properties[TransportHeaderNames.MessageType] = jsonEvent.GetValue("MessageType").ToString();
                eventData.Properties[TransportHeaderNames.MessageName] = jsonEvent.GetValue("MessageName").ToString();
                eventData.Properties[TransportHeaderNames.MessageId] = jsonEvent.GetValue("MessageId").ToString();
                eventData.Properties[TransportHeaderNames.ContentType] = "application/json";

                await eventHubConsumer.ConsumeAsync(eventData, CancellationToken.None);
            }
        }
    }
}