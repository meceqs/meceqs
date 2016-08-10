using System.Threading.Tasks;
using CustomerContext.Contracts.Commands;
using CustomerContext.Contracts.Queries;
using Meceqs;
using Meceqs.Consuming;
using Microsoft.AspNetCore.Mvc;

namespace CustomerContext.WebApi.Controllers
{
    [Route("customers/[action]")]
    [Produces("application/json")]
    public class CustomerController : Controller
    {
        // Note: [FromBody] is not necessary, because we use a global convention (BindComplexTypeFromBodyConvention)
        // Note: Checking for ModelState.IsValid is not necessary, because we use a global filter (RejectInvalidModelStateActionFilter)

        private readonly IEnvelopeConsumer _consumer;

        public CustomerController(IEnvelopeConsumer consumer)
        {
            _consumer = consumer;
        }

        [HttpPost]
        public Task<CreateCustomerResult> CreateCustomer(Envelope<CreateCustomerCommand> envelope)
        {
            return _consumer.ForEnvelope(envelope).ConsumeAsync<CreateCustomerResult>();
        }

        [HttpPost]
        public Task<FindCustomersResult> FindCustomers(Envelope<FindCustomersQuery> envelope)
        {
            return _consumer.ForEnvelope(envelope).ConsumeAsync<FindCustomersResult>();
        }

        [HttpPost]
        public Task<CustomerDto> GetCustomer(Envelope<GetCustomerQuery> envelope)
        {
            return _consumer.ForEnvelope(envelope).ConsumeAsync<CustomerDto>();
        }
    }
}