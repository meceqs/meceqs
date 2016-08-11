using System.Threading.Tasks;

namespace Meceqs.Filters.TypedHandling
{
    public interface IHandleInterceptor
    {
        Task OnHandleExecuting(HandleContext context);

        Task OnHandleExecuted(HandleContext context);
    }
}