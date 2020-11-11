using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Meceqs.Serialization;
using Microsoft.Extensions.Options;

namespace Meceqs.HttpSender
{
    public class HttpSenderMiddleware
    {
        private readonly IOptionsMonitor<HttpSenderOptions> _optionsMonitor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpRequestMessageConverter _httpRequestMessageConverter;
        private readonly ISerializationProvider _serializationProvider;

        public HttpSenderMiddleware(
            MiddlewareDelegate next,
            IOptionsMonitor<HttpSenderOptions> optionsMonitor,
            IHttpClientFactory httpClientFactory,
            IHttpRequestMessageConverter httpRequestMessageConverter,
            ISerializationProvider serializationProvider)
        {
            Guard.NotNull(optionsMonitor, nameof(optionsMonitor));
            Guard.NotNull(httpClientFactory, nameof(httpClientFactory));
            Guard.NotNull(httpRequestMessageConverter, nameof(httpRequestMessageConverter));
            Guard.NotNull(serializationProvider, nameof(serializationProvider));

            // "next" is not stored because this is a terminal middleware.
            _optionsMonitor = optionsMonitor;
            _httpClientFactory = httpClientFactory;
            _httpRequestMessageConverter = httpRequestMessageConverter;
            _serializationProvider = serializationProvider;
        }

        public async Task Invoke(MessageContext context)
        {
            Guard.NotNull(context, nameof(context));

            string pipelineName = context.PipelineName;

            var options = _optionsMonitor.Get(pipelineName);
            var httpClient = _httpClientFactory.CreateClient("Meceqs.HttpSender." + pipelineName);

            Uri absoluteUri = GetAbsoluteRequestUri(options, context.MessageType);

            var request = _httpRequestMessageConverter.ConvertToRequestMessage(context.Envelope, absoluteUri);

            // Gives DelegatingHandlers a chance to access the Meceqs context
            request.Options.TryAdd("Meceqs-MessageContext", context);

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.Cancellation);

            response.EnsureSuccessStatusCode();

            if (context.ExpectedResponseType == typeof(Stream))
            {
                // Special case for downloading files etc.
                // TODO we should probably do this somehow differently
                context.Response = await response.Content.ReadAsStreamAsync();
            }
            else if (context.ExpectedResponseType != typeof(void))
            {
                string contentType = response.Content.Headers.ContentType?.ToString();
                if (!string.IsNullOrEmpty(contentType))
                {
                    var contentTypeList = new List<string> { contentType };

                    var serializer = _serializationProvider.GetSerializer(context.ExpectedResponseType, contentTypeList);

                    context.Response = serializer.Deserialize(context.ExpectedResponseType, await response.Content.ReadAsStreamAsync());
                }
            }
        }

        private static Uri GetAbsoluteRequestUri(HttpSenderOptions options, Type messageType)
        {
            Guard.NotNull(options.BaseAddress, nameof(options.BaseAddress));

            string absoluteUri = options.BaseAddress.TrimEnd('/') + "/";

            absoluteUri += options.MessageConvention.GetRelativePath(messageType).TrimStart('/');

            return new Uri(absoluteUri);
        }
    }
}
