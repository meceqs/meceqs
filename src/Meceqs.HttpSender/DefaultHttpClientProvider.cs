using System.Collections.Generic;
using System.Net.Http;

namespace Meceqs.HttpSender
{
    public class DefaultHttpClientProvider : IHttpClientProvider
    {
        private readonly Dictionary<string, HttpClient> _clients = new Dictionary<string, HttpClient>();

        public void AddHttpClient(string endpointName, HttpClient client)
        {
            Check.NotNullOrWhiteSpace(endpointName, nameof(endpointName));
            Check.NotNull(client, nameof(client));

            _clients.Add(endpointName, client);
        }


        public HttpClient GetHttpClient(string endpointName)
        {
            Check.NotNullOrWhiteSpace(endpointName, nameof(endpointName));

            return _clients[endpointName];
        }

        public void Dispose()
        {
            if (_clients == null)
                return;

            foreach (var client in _clients.Values)
            {
                client.Dispose();
            }
        }
    }
}