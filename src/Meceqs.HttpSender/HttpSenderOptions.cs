using Meceqs.Transport;

namespace Meceqs.HttpSender
{
    public class HttpSenderOptions : SendTransportOptions
    {
        /// <summary>
        /// Gets or sets the base address used for the underlying <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// Configures the mapping between message types and endpoint URIs.
        /// </summary>
        public IEndpointMessageConvention MessageConvention { get; set; } = new DefaultEndpointMessageConvention();
    }
}