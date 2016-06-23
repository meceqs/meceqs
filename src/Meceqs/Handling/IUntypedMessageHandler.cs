using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public interface IUntypedMessageHandler
    {
        Task HandleAsync(Envelope envelope, CancellationToken cancellation);
    }
}