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
            Check.NotNullOrWhiteSpace(endpoint, nameof(endpoint));

            if (Endpoints.ContainsKey(endpoint))
            {
                options?.Invoke(Endpoints[endpoint]);
            }
            else
            {
                var optionsInstance = new EndpointOptions();
                options?.Invoke(optionsInstance);

                Endpoints.Add(endpoint, optionsInstance);
            }
        }
    }
}