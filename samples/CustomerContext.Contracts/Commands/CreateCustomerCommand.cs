using Meceqs;

namespace CustomerContext.Contracts.Commands
{
    public class CreateCustomerCommand : ICommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}