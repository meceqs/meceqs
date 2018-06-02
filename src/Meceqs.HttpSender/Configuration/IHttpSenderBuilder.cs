using Meceqs.HttpSender;
using Meceqs.Transport;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IHttpSenderBuilder : ITransportSenderBuilder<IHttpSenderBuilder, HttpSenderOptions>
    {
        IHttpClientBuilder HttpClient { get; }
    }
}