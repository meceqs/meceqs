using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Channels;
using Meceqs.Pipeline;

namespace Meceqs.Consuming.Internal
{
    public class DefaultConsumeBuilder : IConsumeBuilder
    {
        private const string ChannelName = "Consume";

        private readonly IFilterContextFactory _filterContextFactory;
        private readonly IChannel _channel;
        private readonly IList<Envelope> _envelopes;
        private readonly FilterContextItems _contextItems = new FilterContextItems();

        private CancellationToken _cancellation = CancellationToken.None;

        public DefaultConsumeBuilder(
            IList<Envelope> envelopes,
            IFilterContextFactory filterContextFactory,
            IChannel channel)
        {
            Check.NotNull(envelopes, nameof(envelopes));
            Check.NotNull(filterContextFactory, nameof(filterContextFactory));
            Check.NotNull(channel, nameof(channel));

            _envelopes = envelopes;
            _filterContextFactory = filterContextFactory;
            _channel = channel;
        }

        public IConsumeBuilder SetCancellationToken(CancellationToken cancellation)
        {
            _cancellation = cancellation;
            return this;
        }

        public IConsumeBuilder SetContextItem(string key, object value)
        {
            _contextItems.Set(key, value);
            return this;
        }

        public Task ConsumeAsync()
        {
            return ConsumeAsync<VoidType>();
        }

        public Task<TResult> ConsumeAsync<TResult>()
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