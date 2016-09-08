using System;
using System.Threading.Tasks;

namespace Meceqs.TypedHandling.Internal
{
    public interface IHandlerInvoker
    {
        Task<object> InvokeHandleAsync(IHandles handler, HandleContext handleContext, Type resultType);
    }
}