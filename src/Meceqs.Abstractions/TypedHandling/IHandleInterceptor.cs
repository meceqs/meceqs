using System.Threading.Tasks;

namespace Meceqs.TypedHandling
{
    public interface IHandleInterceptor
    {
        Task OnHandleExecutionAsync(HandleContext context, HandleExecutionDelegate next);
    }
}