using System;

namespace CustomerContext.Contracts.Events
{
    public class CustomerCreatedEvent : IEvent
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public CustomerCreatedEvent() {}

        public CustomerCreatedEvent(Guid customerId, string firstName, string lastName)
        {
            CustomerId = customerId;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}