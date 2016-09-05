using System.Collections.Generic;

namespace Customers.Contracts.Queries
{
    public class FindCustomersResult
    {
        public List<CustomerDto> Customers { get; set; } = new List<CustomerDto>();
    }
}