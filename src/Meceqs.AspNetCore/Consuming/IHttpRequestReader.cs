using System;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Consuming
{
    public interface IHttpRequestReader
    {
        Envelope ConvertToEnvelope(HttpContext httpContext, Type messageType);
    }
}