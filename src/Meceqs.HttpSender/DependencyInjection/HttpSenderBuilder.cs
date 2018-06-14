using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.HttpSender.DependencyInjection
{
    public class HttpSenderBuilder : TransportSenderBuilder<IHttpSenderBuilder, HttpSenderOptions>, IHttpSenderBuilder
    {
        public override IHttpSenderBuilder Instance => this;

        public IHttpClientBuilder HttpClient { get; }

        public HttpSenderBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName)
        {
            HttpClient = meceqsBuilder.Services.AddHttpClient("Meceqs.HttpSender." + PipelineName);

            ConfigurePipeline(pipeline => pipeline.EndsWith(x => x.RunHttpSender()));
        }
    }
}
