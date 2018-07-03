using System;
using System.Net.Http;

namespace Meceqs.HttpSender
{
    public interface IHttpRequestMessageConverter
    {
        HttpRequestMessage ConvertToRequestMessage(Envelope envelope, Uri requestUri);
    }
}