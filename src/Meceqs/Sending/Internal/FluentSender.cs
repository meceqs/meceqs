using System.Collections.Generic;
using System.Threading.Tasks;
using Meceqs.Configuration;
using Meceqs.Pipeline;

namespace Meceqs.Sending.Internal
{
    public class FluentSender : FilterContextBuilder<IFluentSender>, IFluentSender
    {
        private readonly IEnvelopeCorrelator _envelopeCorrelator;

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

        public IFluentSender SetHeader(string headerName, object value)
        {
            foreach (var envelope in Envelopes)
            {
                envelope.Headers[headerName] = value;
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

        protected override IFluentSender GetInstance()
        {
            return this;
        }
    }
}