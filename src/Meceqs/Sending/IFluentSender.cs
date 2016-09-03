using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Sending
{
    public interface IFluentSender : IFilterContextBuilder<IFluentSender>
    {
        IFluentSender CorrelateWith(Envelope source);

        IFluentSender SetHeader(string headerName, object value);

        Task SendAsync();

        Task<TResult> SendAsync<TResult>();
    }
}