using System;

namespace CustomerContext.Contracts.Queries
{
    public class GetCustomerQuery
    {
        public Guid CustomerId { get; set; }
    }
}