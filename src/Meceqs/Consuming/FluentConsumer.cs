using System;
using System.Collections.Generic;
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

        public FluentConsumer(IList<Envelope> envelopes, IServiceProvider serviceProvider)
            : base(MeceqsDefaults.ConsumePipelineName, envelopes, serviceProvider)
        {
        }

        public Task ConsumeAsync()
        {
            return ProcessAsync();
        }

        public Task<TResult> ConsumeAsync<TResult>()
        {
            return ProcessAsync<TResult>();
        }

        public Task<object> ConsumeAsync(Type resultType)
        {
            return ProcessAsync(resultType);
        }
    }
}