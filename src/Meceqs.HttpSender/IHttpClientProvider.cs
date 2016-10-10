using System;
using System.Net.Http;

namespace Meceqs.HttpSender
{
    public interface IHttpClientProvider : IDisposable
    {
        void AddHttpClient(string endpointName, HttpClient client);

        /// <summary>
        /// Returns the previously stored <see cref="HttpClient"/> for the given <paramref name="endpointName"/>.
        /// </summary>
        HttpClient GetHttpClient(string endpointName);
    }
}