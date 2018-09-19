using System.Threading.Tasks;
using Customers.Contracts.Events;
using Meceqs.TypedHandling;
using Microsoft.Extensions.Logging;

namespace Sales.Hosts.ProcessCustomerEvents
{
    public class CustomerEventsHandler :
        IHandles<CustomerCreatedEvent>
    {
        private readonly ILogger _logger;

        public CustomerEventsHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CustomerEventsHandler>();
        }

        public Task HandleAsync(CustomerCreatedEvent msg, HandleContext context)
        {
            _logger.LogInformation("Customer created: {CustomerId} - {FirstName} {LastName}",
                msg.CustomerId, msg.FirstName, msg.LastName);

            return Task.CompletedTask;
        }
    }
}
