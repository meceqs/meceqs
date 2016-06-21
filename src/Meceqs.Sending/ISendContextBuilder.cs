using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface ISendContextBuilder
    {
        ISendContextBuilder CorrelateWith(MessageEnvelope source);

        ISendContextBuilder SetHeader(string headerName, object value);

        ISendContextBuilder SetSendProperty(string key, object value);

        Task<TResult> SendAsync<TResult>();
    }
}