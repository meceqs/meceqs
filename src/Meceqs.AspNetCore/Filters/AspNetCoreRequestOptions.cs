using System;
using System.Reflection;

namespace Meceqs.AspNetCore.Filters
{
    public class AspNetCoreRequestOptions
    {
        public string HostName { get; set; } = Environment.MachineName;

        public string EndpointName { get; set; } = Assembly.GetEntryAssembly().GetName().Name;

        public string HistoryPropertyRequestId { get; set; } = "RequestId";
        public string HistoryPropertyRequestPath { get; set; } = "RequestPath";

        public bool AddRemoteUserHeaders { get; set; } = false;
        public string RemoteUserIpAddressHeaderName { get; set; } = "RemoteIpAddress";
        public string RemoteUserAgentHeaderName { get; set; } = "RemoteUserAgent";
    }
}