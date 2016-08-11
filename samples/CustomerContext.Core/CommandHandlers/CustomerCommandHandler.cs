using System;
using System.Threading.Tasks;
using CustomerContext.Contracts.Commands;
using CustomerContext.Core.Domain;
using CustomerContext.Core.Repositories;
using Meceqs;
using Meceqs.Filters.TypedHandling;
using Meceqs.Sending;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CustomerContext.Core.CommandHandlers
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

        public async Task<CreateCustomerResult> HandleAsync(HandleContext<CreateCustomerCommand> context)
        {
            _logger.LogInformation("MessageName:{MessageName} MessageId:{MessageId}", context.Message.GetType(), context.Envelope.MessageId);

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

            await _sender.ForEvents(events, sourceMessage).SendAsync();

            customer.ClearChanges();
        }
    }
}