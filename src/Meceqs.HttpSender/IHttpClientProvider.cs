using System;
using System.Net.Http;

namespace Meceqs.HttpSender
{
    public interface IHttpClientProvider : IDisposable
    {
        void AddHttpClient(string endpointName, HttpClient client);

        HttpClient GetHttpClient(string endpointName);
    }
}