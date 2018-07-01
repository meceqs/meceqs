using System;
using System.Threading;
using System.Threading.Tasks;
using Customers.Contracts.Commands;
using Customers.Contracts.Queries;
using Meceqs.Sending;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TrafficGenerator
{
    public class TrafficGeneratorService : IHostedService
    {
        private static readonly Random _random = new Random();

        private readonly IServiceProvider _applicationServices;
        private readonly ILogger _logger;

        private Timer _createCustomerTimer;
        private Timer _changeNameTimer;
        private Timer _countCustomersTimer;
        private Timer _placeOrderTimer;

        public TrafficGeneratorService(IServiceProvider applicationServices, ILoggerFactory loggerFactory)
        {
            _applicationServices = applicationServices;
            _logger = loggerFactory.CreateLogger<TrafficGeneratorService>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
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

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _createCustomerTimer?.Dispose();
            _changeNameTimer?.Dispose();
            _countCustomersTimer?.Dispose();
            _placeOrderTimer?.Dispose();
            return Task.CompletedTask;
        }

        private async Task CreateCustomer(IServiceProvider requestServices)
        {
            var messageSender = requestServices.GetRequiredService<IMessageSender>();

            var createCustomerCommand = CustomerFactory.GetRandomCreateCustomerCommand();

            var response = await messageSender.SendAsync<CreateCustomerResponse>(createCustomerCommand);

            _logger.LogInformation("Customer created with id {CustomerId}", response.CustomerId);
        }

        private async Task ChangeName(IServiceProvider requestServices)
        {
            var messageSender = requestServices.GetRequiredService<IMessageSender>();

            // Change name of a random customer.

            var response = await messageSender.ForMessage(new FindCustomersQuery())
                .SendAsync<FindCustomersResponse>();

            var index = new Random().Next(response.Customers.Count);
            Guid customerId = response.Customers[index].CustomerId;

            var changeNameCommand = CustomerFactory.SetRandomName(customerId);

            await messageSender.SendAsync(changeNameCommand);

            _logger.LogInformation("Customer {CustomerId} changed his/her name to {FirstName} {LastName}",
                changeNameCommand.CustomerId,
                changeNameCommand.FirstName,
                changeNameCommand.LastName);
        }

        private async Task CountCustomers(IServiceProvider requestServices)
        {
            var messageSender = requestServices.GetRequiredService<IMessageSender>();

            var response = await messageSender.SendAsync<FindCustomersResponse>(new FindCustomersQuery());

            _logger.LogInformation("Customer-Count: {CustomerCount}", response.Customers.Count);
        }

        private async Task PlaceOrder(IServiceProvider requestServices)
        {
            var messageSender = requestServices.GetRequiredService<IMessageSender>();

            // Place order for a random customer.
            var response = await messageSender.SendAsync<FindCustomersResponse>(new FindCustomersQuery());

            var index = new Random().Next(response.Customers.Count);
            Guid customerId = response.Customers[index].CustomerId;

            var placeOrderCommand = OrderFactory.CreateOrder(customerId);

            await messageSender.ForMessage(placeOrderCommand)
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