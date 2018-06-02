namespace Meceqs.AspNetCore
{
    public class AspNetCoreEnricherOptions
    {
        public bool AddRemoteUserHeaders { get; set; } = false;
        public string RemoteUserIpAddressHeaderName { get; set; } = "RemoteIpAddress";
        public string RemoteUserAgentHeaderName { get; set; } = "RemoteUserAgent";
    }
}