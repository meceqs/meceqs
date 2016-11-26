using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meceqs.Configuration;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Sending
{
    public class SendBuilder : MessageContextBuilder<ISendBuilder>, ISendBuilder
    {
        private readonly IEnvelopeCorrelator _envelopeCorrelator;

        public override ISendBuilder Instance => this;

        public SendBuilder(Envelope envelope, IServiceProvider serviceProvider)
            : base(MeceqsDefaults.SendPipelineName, envelope, serviceProvider)
        {
            _envelopeCorrelator = serviceProvider.GetRequiredService<IEnvelopeCorrelator>();
        }

        public SendBuilder(IEnumerable<Envelope> envelopes, IServiceProvider serviceProvider)
            : base(MeceqsDefaults.SendPipelineName, envelopes, serviceProvider)
        {
            _envelopeCorrelator = serviceProvider.GetRequiredService<IEnvelopeCorrelator>();
        }

        public ISendBuilder CorrelateWith(Envelope source)
        {
            _envelopeCorrelator.CorrelateSourceWithTarget(source, FirstEnvelope);

            if (AdditionalEnvelopes != null)
            {
                foreach (var envelope in AdditionalEnvelopes)
                {
                    _envelopeCorrelator.CorrelateSourceWithTarget(source, envelope);
                }
            }

            return this;
        }

        public Task SendAsync()
        {
            return InvokePipelineAsync();
        }

        public Task<TResult> SendAsync<TResult>()
        {
            return InvokePipelineAsync<TResult>();
        }

        public Task<object> SendAsync(Type resultType)
        {
            return InvokePipelineAsync(resultType);
        }
    }
}