using System;
using System.Collections.Generic;
using CustomerContext.Core.Domain;

namespace CustomerContext.Core.Repositories
{
    public interface ICustomerRepository
    {
        void Add(Customer customer);

        IList<Customer> GetAll();

        Customer GetById(Guid id);
    }
}