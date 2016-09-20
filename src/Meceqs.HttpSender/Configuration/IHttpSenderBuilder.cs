using System;
using Meceqs.Transport;

namespace Meceqs.HttpSender.Configuration
{
    public interface IHttpSenderBuilder : ITransportSenderBuilder<IHttpSenderBuilder>
    {
        IHttpSenderBuilder AddEndpoint(string endpointName, Action<EndpointOptions> options);
    }
}