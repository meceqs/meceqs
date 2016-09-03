using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface IFluentSender
    {
        IFluentSender CorrelateWith(Envelope source);

        IFluentSender SetCancellationToken(CancellationToken cancellation);

        IFluentSender SetRequestServices(IServiceProvider requestServices);

        IFluentSender SetHeader(string headerName, object value);

        IFluentSender SetContextItem(string key, object value);

        IFluentSender UsePipeline(string pipelineName);

        Task SendAsync();

        Task<TResult> SendAsync<TResult>();
    }
}