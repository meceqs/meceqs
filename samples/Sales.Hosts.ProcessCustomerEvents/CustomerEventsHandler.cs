using System;
using System.Threading.Tasks;
using Customers.Contracts.Events;
using Meceqs.Filters.TypedHandling;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

        public Task HandleAsync(HandleContext<CustomerCreatedEvent> context)
        {
            var msg = context.Message;

            _logger.LogInformation("Customer created: {CustomerId} - {FirstName} {LastName}",
                msg.CustomerId, msg.FirstName, msg.LastName);

            return Task.CompletedTask;
        }
    }
}