using System.Threading.Tasks;
using CustomerContext.Contracts.Commands;
using CustomerContext.Core.Domain;
using CustomerContext.Core.Repositories;
using Meceqs.Filters.TypedHandling;
using Meceqs.Sending;
using Microsoft.Extensions.Logging;

namespace CustomerContext.Core.CommandHandlers
{
    public class CustomerCommandHandler
        : IHandles<CreateCustomerCommand, CreateCustomerResult>
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

        public async Task<CreateCustomerResult> HandleAsync(HandleContext<CreateCustomerCommand> ctx)
        {
            _logger.LogInformation("MessageName:{MessageName} MessageId:{MessageId}", ctx.Message.GetType(), ctx.Envelope.MessageId);

            var cmd = ctx.Message;

            var customer = new Customer(cmd.FirstName, cmd.LastName);

            _customerRepository.Add(customer);

            // save events

            var events = customer.GetChanges();
            
            await _sender.ForEvents(events, ctx.Envelope).SendAsync();
            
            customer.ClearChanges();

            return new CreateCustomerResult { CustomerId = customer.Id };
        }
    }
}