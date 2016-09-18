using System.Net.Http;

namespace Meceqs.HttpSender
{
    public interface IHttpRequestMessageConverter
    {
        HttpRequestMessage ConvertToRequestMessage(Envelope envelope, string relativePath);
    }
}