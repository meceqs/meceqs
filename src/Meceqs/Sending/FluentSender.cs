using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meceqs.Configuration;
using Meceqs.Pipeline;

namespace Meceqs.Sending
{
    public class FluentSender : FilterContextBuilder<IFluentSender>, IFluentSender
    {
        private readonly IEnvelopeCorrelator _envelopeCorrelator;

        public override IFluentSender Instance => this;

        public FluentSender(
            IList<Envelope> envelopes,
            IEnvelopeCorrelator envelopeCorrelator,
            IFilterContextFactory filterContextFactory,
            IPipelineProvider pipelineProvider)
            : base(MeceqsDefaults.SendPipelineName, envelopes, filterContextFactory, pipelineProvider)
        {
            Check.NotNull(envelopeCorrelator, nameof(envelopeCorrelator));

            _envelopeCorrelator = envelopeCorrelator;
        }

        public IFluentSender CorrelateWith(Envelope source)
        {
            foreach (var envelope in Envelopes)
            {
                _envelopeCorrelator.CorrelateSourceWithTarget(source, envelope);
            }

            return this;
        }

        public Task SendAsync()
        {
            return ProcessAsync();
        }

        public Task<TResult> SendAsync<TResult>()
        {
            return ProcessAsync<TResult>();
        }

        public Task<object> SendAsync(Type resultType)
        {
            return ProcessAsync(resultType);
        }
    }
}