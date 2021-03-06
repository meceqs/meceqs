using System;

namespace Customers.Contracts.Events
{
    public class CustomerNameChangedEvent : ICustomerEvent
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public CustomerNameChangedEvent() {}

        public CustomerNameChangedEvent(Guid customerId, string firstName, string lastName)
        {
            CustomerId = customerId;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
