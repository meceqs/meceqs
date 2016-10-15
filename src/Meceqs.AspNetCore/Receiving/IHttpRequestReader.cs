using System;
using Microsoft.AspNetCore.Http;

namespace Meceqs.AspNetCore.Receiving
{
    public interface IHttpRequestReader
    {
        Envelope ConvertToEnvelope(HttpContext httpContext, Type messageType);
    }
}