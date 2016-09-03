using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CustomerContext.Contracts.Commands;
using CustomerContext.Contracts.Queries;
using Meceqs.Filters.TypedHandling;
using Meceqs.Serialization;
using Newtonsoft.Json;

namespace TrafficGenerator
{
    /// <summary>
    /// This class does the actual HTTP calls to the remote Web API.
    /// </summary>
    public class CustomerContextApiClient :
        IHandles<CreateCustomerCommand, CreateCustomerResult>,
        IHandles<FindCustomersQuery, FindCustomersResult>
    {
        private const string WebApiUrl = "http://localhost:5891/";

        private static readonly HttpClient _client;

        private readonly IEnvelopeSerializer _serializer;

        static CustomerContextApiClient()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(WebApiUrl);
        }

        public CustomerContextApiClient(IEnvelopeSerializer serializer)
        {
            _serializer = serializer;
        }

        public Task<CreateCustomerResult> HandleAsync(HandleContext<CreateCustomerCommand> context)
        {
            return SendRequestAsync<CreateCustomerResult>(context, "customers/CreateCustomer");
        }

        public Task<FindCustomersResult> HandleAsync(HandleContext<FindCustomersQuery> context)
        {
            return SendRequestAsync<FindCustomersResult>(context, "customers/FindCustomers");
        }

        private async Task<TResult> SendRequestAsync<TResult>(HandleContext context, string relativeUrl)
        {
            return (TResult)await SendRequestAsync(context, relativeUrl, typeof(TResult));
        }

        /// <summary>
        /// Serializes the envelope, sends it to the remote API and deserializes the response.
        /// </summary>
        private async Task<object> SendRequestAsync(HandleContext context, string relativeUrl, Type resultType = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUrl);

            string serializedEnvelope = _serializer.SerializeToString(context.Envelope);
            
            request.Content = new StringContent(serializedEnvelope, Encoding.UTF8, _serializer.ContentType);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(_serializer.ContentType));

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            if (resultType != null)
            {
                // The result isn't an envelope so we can't use the envelope serializer.
                string serializedResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject(serializedResponse, resultType);
            }

            return null;
        }
    }
}