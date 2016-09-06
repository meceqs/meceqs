using System.Linq;
using System.Threading.Tasks;
using Meceqs.Filters.TypedHandling;
using Microsoft.Extensions.Logging;
using Sales.Contracts.Commands;

namespace Sales.Hosts.ProcessOrders
{
    public class PlaceOrderCommandHandler : IHandles<PlaceOrderCommand>
    {
        private readonly ILogger _logger;

        public PlaceOrderCommandHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PlaceOrderCommandHandler>();
        }

        public Task HandleAsync(HandleContext<PlaceOrderCommand> context)
        {
            var cmd = context.Message;

            _logger.LogInformation(
                "Received order from customer {CustomerId} with {ItemCount} items and a total of {TotalAmount}",
                cmd.CustomerId,
                cmd.Items.Count,
                cmd.Items.Sum(x => x.TotalAmount));

            return Task.CompletedTask;
        }
    }
}