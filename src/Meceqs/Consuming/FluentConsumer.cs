using System;
using System.Threading.Tasks;
using Meceqs.Configuration;
using Meceqs.Pipeline;

namespace Meceqs.Consuming
{
    public class FluentConsumer : FilterContextBuilder<IFluentConsumer>, IFluentConsumer
    {
        public override IFluentConsumer Instance => this;

        public FluentConsumer(Envelope envelope, IServiceProvider serviceProvider)
            : base(MeceqsDefaults.ConsumePipelineName, envelope, serviceProvider)
        {
        }

        public Task ConsumeAsync()
        {
            return InvokePipelineAsync();
        }

        public Task<TResult> ConsumeAsync<TResult>()
        {
            return InvokePipelineAsync<TResult>();
        }

        public Task<object> ConsumeAsync(Type resultType)
        {
            return InvokePipelineAsync(resultType);
        }
    }
}