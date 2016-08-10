using System;
using Meceqs;

namespace CustomerContext.Contracts.Queries
{
    public class GetCustomerQuery : IQuery
    {
        public Guid CustomerId { get; set; }
    }
}