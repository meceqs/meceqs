using System.Collections.Generic;

namespace Customers.Contracts.Queries
{
    public class FindCustomersResponse
    {
        public List<CustomerDto> Customers { get; set; } = new List<CustomerDto>();
    }
}