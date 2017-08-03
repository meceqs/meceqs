using System;
using System.Collections.Generic;
using Meceqs.Transport;

namespace Meceqs.HttpSender
{
    public class HttpSenderOptions : TransportSenderOptions
    {
        public Dictionary<string, EndpointOptions> Endpoints { get; set; } = new Dictionary<string, EndpointOptions>();

        public void AddEndpoint(string endpoint, Action<EndpointOptions> options)
        {
            Guard.NotNullOrWhiteSpace(endpoint, nameof(endpoint));
            Guard.NotNull(options, nameof(options));

            EndpointOptions existingEndpoint;
            if (Endpoints.TryGetValue(endpoint, out existingEndpoint))
            {
                options(existingEndpoint);
            }
            else
            {
                var endpointOptions = new EndpointOptions();
                options(endpointOptions);

                Endpoints.Add(endpoint, endpointOptions);
            }
        }
    }
}