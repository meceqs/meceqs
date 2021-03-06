using System.Linq;
using System.Threading.Tasks;
using Customers.Contracts.Queries;
using Customers.Core.Domain;
using Customers.Core.Repositories;
using Meceqs.TypedHandling;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Customers.Core.QueryHandlers
{
    [CustomLogic /* this attribute can be read by an IHandleInterceptor */]
    public class CustomerQueryHandler :
        IHandles<FindCustomersQuery, FindCustomersResponse>,
        IHandles<GetCustomerQuery, CustomerDto>
    {
        private readonly ILogger _logger;
        private readonly ICustomerRepository _customerRepository;

        public CustomerQueryHandler(ILoggerFactory loggerFactory, ICustomerRepository customerRepository)
        {
            _logger = loggerFactory.CreateLogger<CustomerQueryHandler>();

            _customerRepository = customerRepository;
        }

        public Task<FindCustomersResponse> HandleAsync(FindCustomersQuery qry, HandleContext context)
        {
            _logger.LogInformation("FindCustomers - MessageType:{MessageType} MessageId:{MessageId}",
                context.Message.GetType(), context.Envelope.MessageId);

            _logger.LogInformation("Envelope:{Envelope}", JsonConvert.SerializeObject(context.Envelope));

            var customers = _customerRepository.GetAll();

            var response = new FindCustomersResponse
            {
                Customers = customers.Select(x => ToDto(x)).ToList()
            };

            return Task.FromResult(response);
        }

        public Task<CustomerDto> HandleAsync(GetCustomerQuery qry, HandleContext context)
        {
            _logger.LogInformation("GetCustomer - MessageType:{MessageType} MessageId:{MessageId}",
                context.Message.GetType(), context.Envelope.MessageId);

            var customer = _customerRepository.GetById(qry.CustomerId);

            return Task.FromResult(ToDto(customer));
        }

        private static CustomerDto ToDto(Customer customer)
        {
            return new CustomerDto
            {
                CustomerId = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName
            };
        }
    }
}
