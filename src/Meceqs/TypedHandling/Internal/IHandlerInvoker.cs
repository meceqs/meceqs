using System;
using System.Threading.Tasks;

namespace Meceqs.TypedHandling.Internal
{
    public interface IHandlerInvoker
    {
        Task InvokeHandleAsync(HandleContext handleContext);
    }
}
