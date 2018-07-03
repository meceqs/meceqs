using Meceqs;
using Meceqs.HttpSender;
using Meceqs.Transport;

namespace Microsoft.Extensions.DependencyInjection
{
    public class HttpSenderBuilder : SendTransportBuilder<HttpSenderBuilder, HttpSenderOptions>
    {
        protected override HttpSenderBuilder Instance => this;

        public IHttpClientBuilder HttpClient { get; }

        public HttpSenderBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName)
        {
            HttpClient = meceqsBuilder.Services.AddHttpClient("Meceqs.HttpSender." + PipelineName);

            ConfigurePipeline(pipeline => pipeline.EndsWith(x => x.RunHttpSender()));
        }

        public HttpSenderBuilder SetBaseAddress(string baseAddress)
        {
            Guard.NotNull(baseAddress, nameof(baseAddress));

            return Configure(options => options.BaseAddress = baseAddress);
        }
    }
}
