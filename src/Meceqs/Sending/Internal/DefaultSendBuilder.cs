using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Channels;
using Meceqs.Pipeline;

namespace Meceqs.Sending.Internal
{
    public class DefaultSendBuilder : ISendBuilder
    {
        private const string ChannelName = "Send";

        private readonly IEnvelopeCorrelator _envelopeCorrelator;
        private readonly IFilterContextFactory _filterContextFactory;
        private readonly IChannel _channel;

        private readonly IList<Envelope> _envelopes;
        private readonly FilterContextItems _contextItems = new FilterContextItems();

        private CancellationToken _cancellation = CancellationToken.None;

        public DefaultSendBuilder(
            IList<Envelope> envelopes,
            IEnvelopeCorrelator envelopeCorrelator,
            IFilterContextFactory filterContextFactory,
            IChannel channel)
        {
            Check.NotNull(envelopes, nameof(envelopes));
            Check.NotNull(envelopeCorrelator, nameof(envelopeCorrelator));
            Check.NotNull(filterContextFactory, nameof(filterContextFactory));
            Check.NotNull(channel, nameof(channel));

            _envelopes = envelopes;
            _envelopeCorrelator = envelopeCorrelator;
            _filterContextFactory = filterContextFactory;
            _channel = channel;
        }

        public ISendBuilder CorrelateWith(Envelope source)
        {
            foreach (var envelope in _envelopes)
            {
                _envelopeCorrelator.CorrelateSourceWithTarget(source, envelope);
            }

            return this;
        }

        public ISendBuilder SetCancellationToken(CancellationToken cancellation)
        {
            _cancellation = cancellation;
            return this;
        }

        public ISendBuilder SetHeader(string headerName, object value)
        {
            foreach (var envelope in _envelopes)
            {
                envelope.SetHeader(headerName, value);
            }

            return this;
        }

        public ISendBuilder SetContextItem(string key, object value)
        {
            _contextItems.Set(key, value);
            return this;
        }

        public Task SendAsync()
        {
            return SendAsync<VoidType>();
        }

        public Task<TResult> SendAsync<TResult>()
        {
            var filterContexts = _envelopes.Select(CreateFilterContext).ToList();
            
            return _channel.SendAsync<TResult>(filterContexts);
        }

        private FilterContext CreateFilterContext(Envelope envelope)
        {
            var context = _filterContextFactory.CreateFilterContext(envelope);

            context.Cancellation = _cancellation;
            context.ChannelName = ChannelName;

            if (_contextItems.Count > 0)
            {
                foreach (var kvp in _contextItems)
                {
                    context.SetContextItem(kvp.Key, kvp.Value);
                }
            }

            return context;
        }
    }
}