using System;

namespace CustomerContext.Contracts.Events
{
    public interface ICustomerEvent
    {
        Guid CustomerId { get; set; }
    }
}