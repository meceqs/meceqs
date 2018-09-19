using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Meceqs.Receiving
{
    public class ReceiveBuilder : MessageContextBuilder<IReceiveBuilder>, IReceiveBuilder
    {
        public override IReceiveBuilder Instance => this;

        public ReceiveBuilder(Envelope envelope, IServiceProvider serviceProvider)
            : base(MeceqsDefaults.ReceivePipelineName, envelope, serviceProvider)
        {
        }

        public Task ReceiveAsync()
        {
            return InvokePipelineAsync();
        }

        public Task<TResponse> ReceiveAsync<TResponse>()
        {
            return InvokePipelineAsync<TResponse>();
        }

        public Task<object> ReceiveAsync(Type responseType)
        {
            return InvokePipelineAsync(responseType);
        }
    }
}
