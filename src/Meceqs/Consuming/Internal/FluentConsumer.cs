using System.Collections.Generic;
using System.Threading.Tasks;
using Meceqs.Configuration;
using Meceqs.Pipeline;

namespace Meceqs.Consuming.Internal
{
    public class FluentConsumer : FilterContextBuilder<IFluentConsumer>, IFluentConsumer
    {
        public FluentConsumer(
            IList<Envelope> envelopes,
            IFilterContextFactory filterContextFactory,
            IPipelineProvider pipelineProvider)
            : base(MeceqsDefaults.ConsumePipelineName, envelopes, filterContextFactory, pipelineProvider)
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

        protected override IFluentConsumer GetInstance()
        {
            return this;
        }
    }
}