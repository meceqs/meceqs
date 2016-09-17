using System;
using System.Security.Claims;
using System.Threading;

namespace Meceqs.Pipeline
{
    public interface IFilterContextBuilder<TBuilder>
        where TBuilder : IFilterContextBuilder<TBuilder>
    {
        TBuilder Instance { get; }

        TBuilder SetCancellationToken(CancellationToken cancellation);

        TBuilder SetContextItem(string key, object value);

        TBuilder SetHeader(string headerName, object value);

        TBuilder SetRequestServices(IServiceProvider requestServices);

        TBuilder SetUser(ClaimsPrincipal user);

        TBuilder UsePipeline(string pipelineName);
    }
}