using System;
using System.Threading.Tasks;

namespace Meceqs.Filters.TypedHandling.Internal
{
    public interface IHandlerInvoker
    {
        Task<object> InvokeHandleAsync(object handler, HandleContext handleContext, Type resultType);
    }
}