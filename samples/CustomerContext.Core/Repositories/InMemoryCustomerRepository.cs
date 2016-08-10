using System;
using System.Collections.Generic;
using System.Linq;
using CustomerContext.Core.Domain;

namespace CustomerContext.Core.Repositories
{
    public class InMemoryCustomerRepository : ICustomerRepository
    {
        private readonly List<Customer> _customers = new List<Customer>() 
        {
            new Customer("Evan", "Manning"),
            new Customer("Jennifer", "Wallace"),
            new Customer("Gavin", "McGrath"),
            new Customer("Joe", "Gray"),
            new Customer("Connor", "McGrath")
        };

        public void Add(Customer customer)
        {
            _customers.Add(customer);
        }

        public IList<Customer> GetAll()
        {
            return _customers.ToList();
        }

        public Customer GetById(Guid id)
        {
            return _customers.FirstOrDefault(x => x.Id == id);
        }
    }
}