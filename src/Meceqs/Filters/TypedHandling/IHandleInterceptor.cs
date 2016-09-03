using System.Threading.Tasks;

namespace Meceqs.Filters.TypedHandling
{
    public interface IHandleInterceptor
    {
        Task OnHandleExecutionAsync(HandleContext context, HandleExecutionDelegate next);
    }
}