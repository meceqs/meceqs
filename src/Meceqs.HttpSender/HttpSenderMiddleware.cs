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
        private readonly ISerializationProvider _serializationProvider;

        private readonly Dictionary<Type, Tuple<string, EndpointMessage>> _messageMapping;

        public HttpSenderMiddleware(
            MiddlewareDelegate next,
            IOptions<HttpSenderOptions> options,
            IServiceProvider serviceProvider,
            IHttpClientProvider httpClientProvider,
            IHttpRequestMessageConverter httpRequestMessageConverter,
            ISerializationProvider serializationProvider)
        {
            Guard.NotNull(options?.Value, nameof(options));
            Guard.NotNull(serviceProvider, nameof(serviceProvider));
            Guard.NotNull(httpClientProvider, nameof(httpClientProvider));
            Guard.NotNull(httpRequestMessageConverter, nameof(httpRequestMessageConverter));
            Guard.NotNull(serializationProvider, nameof(serializationProvider));

            // "next" is not stored because this is a terminal middleware.
            _options = options.Value;
            _serviceProvider = serviceProvider;
            _httpClientProvider = httpClientProvider;
            _httpRequestMessageConverter = httpRequestMessageConverter;
            _serializationProvider = serializationProvider;

            _messageMapping = BuildMessageMapping();

            CreateHttpClients();
        }

        public async Task Invoke(MessageContext context)
        {
            Guard.NotNull(context, nameof(context));

            Tuple<string, EndpointMessage> mapping;
            if (!_messageMapping.TryGetValue(context.MessageType, out mapping))
            {
                throw new InvalidOperationException($"No endpoint found for message type '{context.MessageType}'");
            }

            var httpClient = _httpClientProvider.GetHttpClient(mapping.Item1);

            var request = _httpRequestMessageConverter.ConvertToRequestMessage(context.Envelope, mapping.Item2.RelativePath);

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, context.Cancellation);

            response.EnsureSuccessStatusCode();

            if (context.ExpectedResultType != typeof(void))
            {
                string contentType = response.Content.Headers.ContentType.ToString();
                if (!_serializationProvider.TryGetSerializer(contentType, out ISerializer serializer))
                {
                    throw new NotSupportedException($"ContentType '{contentType}' is not supported.");
                }

                context.Result = serializer.Deserialize(context.ExpectedResultType, await response.Content.ReadAsStreamAsync());
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