using System.Threading.Tasks;
using Customers.Contracts.Commands;
using Customers.Contracts.Queries;
using Meceqs;
using Meceqs.Consuming;
using Microsoft.AspNetCore.Mvc;

namespace Customers.Hosts.WebApi.Controllers
{
    [Route("customers/[action]")]
    [Produces("application/json")]
    public class CustomerController : Controller
    {
        // Note: [FromBody] is not necessary, because we use a global convention (BindComplexTypeFromBodyConvention)
        // Note: Checking for ModelState.IsValid is not necessary, because we use a global filter (RejectInvalidModelStateActionFilter)

        private readonly IMessageConsumer _messageConsumer;

        public CustomerController(IMessageConsumer messageConsumer)
        {
            _messageConsumer = messageConsumer;
        }

        [HttpPost]
        public Task<CreateCustomerResult> CreateCustomer(Envelope<CreateCustomerCommand> envelope)
        {
            return _messageConsumer.ConsumeAsync<CreateCustomerResult>(envelope);
        }

        [HttpPost]
        public Task ChangeName(Envelope<ChangeNameCommand> envelope)
        {
            return _messageConsumer.ConsumeAsync(envelope);
        }

        [HttpPost]
        public Task<FindCustomersResult> FindCustomers(Envelope<FindCustomersQuery> envelope)
        {
            return _messageConsumer.ConsumeAsync<FindCustomersResult>(envelope);
        }

        [HttpPost]
        public Task<CustomerDto> GetCustomer(Envelope<GetCustomerQuery> envelope)
        {
            return _messageConsumer.ConsumeAsync<CustomerDto>(envelope);
        }
    }
}