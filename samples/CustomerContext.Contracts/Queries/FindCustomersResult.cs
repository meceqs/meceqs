using System.Collections.Generic;

namespace CustomerContext.Contracts.Queries
{
    public class FindCustomersResult
    {
        public List<CustomerDto> Customers { get; set; } = new List<CustomerDto>();
    }
}