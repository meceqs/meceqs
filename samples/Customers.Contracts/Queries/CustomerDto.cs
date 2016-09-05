using System;

namespace CustomerContext.Contracts.Queries
{
    public class CustomerDto
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}