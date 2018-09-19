using System;
using System.Collections.Generic;
using Customers.Contracts.Events;

namespace Customers.Core.Domain
{
    public class Customer
    {
        private readonly IList<ICustomerEvent> _changes = new List<ICustomerEvent>();
        public IList<ICustomerEvent> GetChanges() => _changes;
        public void ClearChanges() => _changes.Clear();


        public Guid Id { get; protected set; }

        public string FirstName { get; protected set; }

        public string LastName { get; protected set; }

        public Customer(string firstName, string lastName)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;

            _changes.Add(new CustomerCreatedEvent(Id, FirstName, LastName));
        }

        public void ChangeName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;

            _changes.Add(new CustomerNameChangedEvent(Id, FirstName, LastName));
        }
    }
}
