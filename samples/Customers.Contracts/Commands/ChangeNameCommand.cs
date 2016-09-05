using System;

namespace CustomerContext.Contracts.Commands
{
    public class ChangeNameCommand
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}