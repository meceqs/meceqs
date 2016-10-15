using System;
using System.Threading.Tasks;
using Meceqs.Configuration;
using Meceqs.Pipeline;

namespace Meceqs.Consuming
{
    public class ConsumeBuilder : FilterContextBuilder<IConsumeBuilder>, IConsumeBuilder
    {
        public override IConsumeBuilder Instance => this;

        public ConsumeBuilder(Envelope envelope, IServiceProvider serviceProvider)
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