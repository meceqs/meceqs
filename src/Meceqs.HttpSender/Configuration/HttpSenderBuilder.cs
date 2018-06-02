using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.HttpSender.Configuration
{
    public class HttpSenderBuilder : TransportSenderBuilder<IHttpSenderBuilder, HttpSenderOptions>, IHttpSenderBuilder
    {
        public override IHttpSenderBuilder Instance => this;

        public IHttpClientBuilder HttpClient { get; }

        public HttpSenderBuilder(IServiceCollection services, string pipelineName)
            : base(services, pipelineName)
        {
            HttpClient = services.AddHttpClient("Meceqs.HttpSender." + pipelineName);
        }
    }
}
