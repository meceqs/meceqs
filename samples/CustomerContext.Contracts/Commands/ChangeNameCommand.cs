using System;
using Meceqs;

namespace CustomerContext.Contracts.Commands
{
    public class ChangeNameCommand : ICommand
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}