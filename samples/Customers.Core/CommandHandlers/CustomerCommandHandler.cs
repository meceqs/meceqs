using System;
using System.Threading.Tasks;
using Customers.Contracts.Commands;
using Customers.Core.Domain;
using Customers.Core.Repositories;
using Meceqs;
using Meceqs.Sending;
using Meceqs.TypedHandling;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Customers.Core.CommandHandlers
{
    public class CustomerCommandHandler :
        IHandles<CreateCustomerCommand, CreateCustomerResult>,
        IHandles<ChangeNameCommand>
    {
        private readonly ILogger _logger;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMessageSender _sender;

        public CustomerCommandHandler(ILoggerFactory loggerFactory, ICustomerRepository customerRepository, IMessageSender sender)
        {
            _logger = loggerFactory.CreateLogger<CustomerCommandHandler>();

            _customerRepository = customerRepository;
            _sender = sender;
        }

        [CustomLogic /* this attribute can be read by an IHandleInterceptor */]
        public async Task<CreateCustomerResult> HandleAsync(HandleContext<CreateCustomerCommand> context)
        {
            _logger.LogInformation("MessageType:{MessageType} MessageId:{MessageId}", context.Message.GetType(), context.Envelope.MessageId);

            _logger.LogInformation("Envelope:{Envelope}", JsonConvert.SerializeObject(context.Envelope));

            var cmd = context.Message;

            var customer = new Customer(cmd.FirstName, cmd.LastName);

            _customerRepository.Add(customer);

            await SaveAndClearEvents(customer, context.Envelope);

            return new CreateCustomerResult { CustomerId = customer.Id };
        }

        public async Task HandleAsync(HandleContext<ChangeNameCommand> context)
        {
            _logger.LogInformation("Envelope:{Envelope}", JsonConvert.SerializeObject(context.Envelope));

            var cmd = context.Message;

            var customer = _customerRepository.GetById(cmd.CustomerId);
            if (customer == null)
                throw new InvalidOperationException("customer not found");

            customer.ChangeName(cmd.FirstName, cmd.LastName);

            await SaveAndClearEvents(customer, context.Envelope);
        }

        private async Task SaveAndClearEvents(Customer customer, Envelope sourceMessage)
        {
            var events = customer.GetChanges();

            await _sender.ForMessages(events)
                .CorrelateWith(sourceMessage)
                .SendAsync();

            customer.ClearChanges();
        }
    }
}