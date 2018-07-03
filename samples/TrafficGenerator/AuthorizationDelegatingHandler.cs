using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrafficGenerator
{
    public class AuthorizationDelegatingHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // This is a simple demo with "Basic" authentication.
            // You should use e.g. an OAuth token instead.

            string authenticationValue = Convert.ToBase64String(Encoding.ASCII.GetBytes("username:password"));

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authenticationValue);

            return base.SendAsync(request, cancellationToken);
        }
    }
}