using System;

namespace Customers.Contracts.Events
{
    public interface ICustomerEvent
    {
        Guid CustomerId { get; set; }
    }
}