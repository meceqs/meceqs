using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Meceqs.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Meceqs.HttpSender
{
    public class HttpSenderMiddleware
    {
        private readonly HttpSenderOptions _options;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpClientProvider _httpClientProvider;
        private readonly IHttpRequestMessageConverter _httpRequestMessageConverter;
        private readonly IResultDeserializer _resultDeserializer;

        private readonly Dictionary<Type, Tuple<string, EndpointMessage>> _messageMapping;

        public HttpSenderMiddleware(
            MessageDelegate next,
            IOptions<HttpSenderOptions> options,
            IServiceProvider serviceProvider,
            IHttpClientProvider httpClientProvider,
            IHttpRequestMessageConverter httpRequestMessageConverter,
            IResultDeserializer resultDeserializer)
        {
            Check.NotNull(options?.Value, nameof(options));
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            Check.NotNull(httpClientProvider, nameof(httpClientProvider));
            Check.NotNull(httpRequestMessageConverter, nameof(httpRequestMessageConverter));
            Check.NotNull(resultDeserializer, nameof(resultDeserializer));

            // "next" is not stored because this is a terminal middleware.
            _options = options.Value;
            _serviceProvider = serviceProvider;
            _httpClientProvider = httpClientProvider;
            _httpRequestMessageConverter = httpRequestMessageConverter;
            _resultDeserializer = resultDeserializer;

            _messageMapping = BuildMessageMapping();

            CreateHttpClients();
        }

        public async Task Invoke(MessageContext context)
        {
            Check.NotNull(context, nameof(context));

            Tuple<string, EndpointMessage> mapping;
            if (!_messageMapping.TryGetValue(context.MessageType, out mapping))
            {
                throw new InvalidOperationException($"No endpoint found for message type '{context.MessageType}'");
            }

            var httpClient = _httpClientProvider.GetHttpClient(mapping.Item1);

            var request = _httpRequestMessageConverter.ConvertToRequestMessage(context.Envelope, mapping.Item2.RelativePath);

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, context.Cancellation);

            response.EnsureSuccessStatusCode();

            if (context.ExpectedResultType != null)
            {
                context.Result = _resultDeserializer.DeserializeResultFromStream(
                    response.Content.Headers.ContentType.MediaType,
                    await response.Content.ReadAsStreamAsync(),
                    context.ExpectedResultType);
            }
        }

        private Dictionary<Type, Tuple<string, EndpointMessage>> BuildMessageMapping()
        {
            var mapping = new Dictionary<Type, Tuple<string, EndpointMessage>>();

            foreach (var endpoint in _options.Endpoints)
            {
                foreach (var message in endpoint.Value.Messages)
                {
                    mapping.Add(message.MessageType, Tuple.Create(endpoint.Key, message));
                }
            }

            return mapping;
        }

        private void CreateHttpClients()
        {
            foreach (var endpoint in _options.Endpoints)
            {
                var endpointName = endpoint.Key;
                var endpointOptions = endpoint.Value;

                var client = CreateHttpClient(endpointOptions.DelegatingHandlers);

                // trailing slash is really important:
                // http://stackoverflow.com/questions/23438416/why-is-httpclient-baseaddress-not-working
                client.BaseAddress = new Uri(endpointOptions.BaseAddress.TrimEnd('/') + "/");

                _httpClientProvider.AddHttpClient(endpointName, client);
            }
        }

        private HttpClient CreateHttpClient(IList<Type> delegatingHandlers)
        {
            // HttpClientHandler is the most-inner handler.
            HttpMessageHandler handler = new HttpClientHandler();

            // Create a chain of all configured handlers (must be created in reverse order)

            foreach (var handlerType in delegatingHandlers.Reverse())
            {
                // This allows dependency injection on handlers.
                var delegatingHandler = (DelegatingHandler)ActivatorUtilities.CreateInstance(_serviceProvider, handlerType);

                delegatingHandler.InnerHandler = handler;

                handler = delegatingHandler;
            }

            return new HttpClient(handler);
        }
    }
}