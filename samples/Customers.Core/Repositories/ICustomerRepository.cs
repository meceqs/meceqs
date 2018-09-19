using System;
using System.Collections.Generic;
using Customers.Core.Domain;

namespace Customers.Core.Repositories
{
    public interface ICustomerRepository
    {
        void Add(Customer customer);

        IList<Customer> GetAll();

        Customer GetById(Guid id);
    }
}
