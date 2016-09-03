using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Sending
{
    public interface IFluentSender : IFilterContextBuilder<IFluentSender>
    {
        IFluentSender CorrelateWith(Envelope source);

        Task SendAsync();

        Task<TResult> SendAsync<TResult>();
    }
}