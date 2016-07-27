using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public class DefaultSendBuilder : ISendBuilder
    {
        private readonly IEnvelopeCorrelator _envelopeCorrelator;
        private readonly IMessageContextFactory _messageContextFactory;
        private readonly IMessageSendingMediator _sendingMediator;

        private readonly IList<Envelope> _envelopes;
        private readonly MessageContextData _contextData = new MessageContextData();

        private CancellationToken _cancellation = CancellationToken.None;

        public DefaultSendBuilder(
            IList<Envelope> envelopes,
            IEnvelopeCorrelator envelopeCorrelator,
            IMessageContextFactory messageContextFactory,
            IMessageSendingMediator sendingMediator)
        {
            Check.NotNull(envelopes, nameof(envelopes));
            Check.NotNull(envelopeCorrelator, nameof(envelopeCorrelator));
            Check.NotNull(messageContextFactory, nameof(messageContextFactory));
            Check.NotNull(sendingMediator, nameof(sendingMediator));

            _envelopes = envelopes;
            _envelopeCorrelator = envelopeCorrelator;
            _messageContextFactory = messageContextFactory;
            _sendingMediator = sendingMediator;
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
            _contextData.Set(key, value);
            return this;
        }

        public Task SendAsync()
        {
            return SendAsync<VoidType>();
        }

        public async Task<TResult> SendAsync<TResult>()
        {
            // TODO @cweiss does this make sense?

            if (_envelopes.Count == 0)
            {
                // TODO @cweiss Should this throw an exception instead?
                return await Task.FromResult(default(TResult));
            }
            else if (_envelopes.Count == 1)
            {
                var envelope = _envelopes.First();
                var context = _messageContextFactory.Create(envelope, _contextData, _cancellation);
                return await _sendingMediator.SendAsync<TResult>(context);
            }
            else
            {
                if (typeof(TResult) != typeof(VoidType))
                    throw new InvalidOperationException("SendAsync with many envelopes can only be used with return-type 'VoidType'");

                foreach (var envelope in _envelopes)
                {
                    var context = _messageContextFactory.Create(envelope, _contextData, _cancellation);
                    await _sendingMediator.SendAsync<TResult>(context);
                }

                return await Task.FromResult(default(TResult));
            }
        }
    }
}