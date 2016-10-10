using System;
using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.HttpSender.Configuration
{
    public class HttpSenderBuilder : TransportSenderBuilder<IHttpSenderBuilder, HttpSenderOptions>, IHttpSenderBuilder
    {
        public override IHttpSenderBuilder Instance => this;

        public HttpSenderBuilder()
        {
            PipelineEndHook = pipeline => pipeline.RunHttpSender();
        }

        public IHttpSenderBuilder AddEndpoint(string endpointName, Action<EndpointOptions> options)
        {
            Check.NotNullOrWhiteSpace(endpointName, nameof(endpointName));
            Check.NotNull(options, nameof(options));

            SenderOptions += o => o.AddEndpoint(endpointName, options);
            return this;
        }
    }
}