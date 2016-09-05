using System.Collections.Generic;
using System.Threading.Tasks;
using Meceqs.Configuration;
using Meceqs.Pipeline;

namespace Meceqs.Consuming.Internal
{
    public class FluentConsumer : FilterContextBuilder<IFluentConsumer>, IFluentConsumer
    {
        public override IFluentConsumer Instance => this;

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
    }
}