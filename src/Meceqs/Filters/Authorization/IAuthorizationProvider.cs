using System;
using System.Threading.Tasks;

namespace Meceqs.Filters.Authorization
{
    public interface IAuthorizationProvider
    {
        Task AuthorizeAsync(Type messageType);
    }
}