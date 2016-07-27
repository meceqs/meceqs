using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface ISendBuilder
    {
        ISendBuilder CorrelateWith(Envelope source);

        ISendBuilder SetCancellationToken(CancellationToken cancellation);

        ISendBuilder SetHeader(string headerName, object value);

        ISendBuilder SetContextItem(string key, object value);

        Task SendAsync();

        Task<TResult> SendAsync<TResult>();
    }
}