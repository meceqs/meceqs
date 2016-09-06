using System;
using System.Threading;
using System.Threading.Tasks;
using Customers.Contracts.Commands;
using Customers.Contracts.Queries;
using Meceqs.Sending;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TrafficGenerator
{
    public class Worker
    {
        private static readonly Random _random = new Random();

        private readonly IServiceProvider _applicationServices;
        private readonly ILogger _logger;

        private Timer _createCustomerTimer;
        private Timer _changeNameTimer;
        private Timer _countCustomersTimer;
        private Timer _placeOrderTimer;

        public Worker(IServiceProvider applicationServices, ILoggerFactory loggerFactory)
        {
            _applicationServices = applicationServices;
            _logger = loggerFactory.CreateLogger<Worker>();
        }

        public void Run()
        {
            _logger.LogInformation("Starting timers");

            bool createCustomerEnabled = true;
            bool changeNameEnabled = true;
            bool countCustomersEnabled = true;
            bool placeOrderEnabled = true;

            if (createCustomerEnabled)
            {
                _createCustomerTimer = new Timer(_ =>
               {
                   InvokeTimerMethod("CreateCustomer", CreateCustomer, _createCustomerTimer, 5 * 1000);
               }, null, _random.Next(1000) /* first start */, Timeout.Infinite);
            }

            if (changeNameEnabled)
            {
                _changeNameTimer = new Timer(_ =>
                {
                    InvokeTimerMethod("ChangeName", ChangeName, _changeNameTimer, 10 * 1000);
                }, null, _random.Next(5000, 8000) /* first start */, Timeout.Infinite);
            }

            if (countCustomersEnabled)
            {
                _countCustomersTimer = new Timer(_ =>
                {
                    InvokeTimerMethod("CountCustomers", CountCustomers, _countCustomersTimer, 2 * 1000);
                }, null, _random.Next(1000) /* first start */, Timeout.Infinite);
            }

            if (placeOrderEnabled)
            {
                _placeOrderTimer = new Timer(_ =>
                {
                    InvokeTimerMethod("PlaceOrder", PlaceOrder, _placeOrderTimer, 4 * 1000);
                }, null, _random.Next(2000, 4000) /* first start */, Timeout.Infinite);
            }
        }

        private async Task CreateCustomer(IServiceProvider requestServices)
        {
            var messageSender = requestServices.GetRequiredService<IMessageSender>();

            var createCustomerCommand = CustomerFactory.GetRandomCreateCustomerCommand();

            var result = await messageSender.ForMessage(createCustomerCommand)
                .SetRequestServices(requestServices)
                .SendAsync<CreateCustomerResult>();

            _logger.LogInformation("Customer created with id {CustomerId}", result.CustomerId);
        }

        private async Task ChangeName(IServiceProvider requestServices)
        {
            var messageSender = requestServices.GetRequiredService<IMessageSender>();

            // Change name of a random customer.

            var result = await messageSender.ForMessage(new FindCustomersQuery())
                .SetRequestServices(requestServices)
                .SendAsync<FindCustomersResult>();

            var index = new Random().Next(result.Customers.Count);
            Guid customerId = result.Customers[index].CustomerId;

            var changeNameCommand = CustomerFactory.SetRandomName(customerId);

            await messageSender.ForMessage(changeNameCommand)
                .SetRequestServices(requestServices)
                .SendAsync();

            _logger.LogInformation("Customer {CustomerId} changed his/her name to {FirstName} {LastName}",
                changeNameCommand.CustomerId,
                changeNameCommand.FirstName,
                changeNameCommand.LastName);
        }

        private async Task CountCustomers(IServiceProvider requestServices)
        {
            var messageSender = requestServices.GetRequiredService<IMessageSender>();

            var result = await messageSender.ForMessage(new FindCustomersQuery())
                .SetRequestServices(requestServices)
                .SendAsync<FindCustomersResult>();

            _logger.LogInformation("Customer-Count: {CustomerCount}", result.Customers.Count);
        }

        private async Task PlaceOrder(IServiceProvider requestServices)
        {
            var messageSender = requestServices.GetRequiredService<IMessageSender>();

            // Place order for a random customer.
            var result = await messageSender.ForMessage(new FindCustomersQuery())
                .SetRequestServices(requestServices)
                .SendAsync<FindCustomersResult>();

            var index = new Random().Next(result.Customers.Count);
            Guid customerId = result.Customers[index].CustomerId;

            var placeOrderCommand = OrderFactory.CreateOrder(customerId);

            await messageSender.ForMessage(placeOrderCommand)
                .SetRequestServices(requestServices)
                .UsePipeline("ServiceBus")
                .SendAsync();
        }

        private void InvokeTimerMethod(string name, Func<IServiceProvider, Task> action, Timer timer, int interval)
        {
            try
            {
                _logger.LogInformation("Executing timer method {Method}", name);

                var serviceScopeFactory = _applicationServices.GetRequiredService<IServiceScopeFactory>();
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    action(scope.ServiceProvider).GetAwaiter().GetResult();
                }
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
    }
}