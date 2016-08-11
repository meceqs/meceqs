using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface IFluentSender
    {
        IFluentSender CorrelateWith(Envelope source);

        IFluentSender SetCancellationToken(CancellationToken cancellation);

        IFluentSender SetHeader(string headerName, object value);

        IFluentSender SetContextItem(string key, object value);

        Task SendAsync();

        Task<TResult> SendAsync<TResult>();
    }
}