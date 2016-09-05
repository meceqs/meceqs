using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.Extensions.Logging;

namespace Customers.Hosts.WebApi.Infrastructure
{
    public class FakeServiceBusFilter
    {
        private readonly ILogger _logger;

        public FakeServiceBusFilter(FilterDelegate next, ILoggerFactory loggerFactory)
        {
            // "next" is not stored because this is a terminating filter.
            _logger = loggerFactory.CreateLogger<FakeServiceBusFilter>();
        }

        public Task Invoke(FilterContext context)
        {
            _logger.LogWarning(
                "Simulating send for {MessageType}/{MessageId}",
                context.MessageType,
                context.Envelope.MessageId);

            return Task.CompletedTask;
        }
    }
}