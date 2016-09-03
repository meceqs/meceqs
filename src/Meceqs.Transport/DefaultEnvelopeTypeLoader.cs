using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Meceqs.Transport
{
    public class DefaultEnvelopeTypeLoader : IEnvelopeTypeLoader
    {
        private readonly MeceqsTransportOptions _options;
        private readonly ConcurrentDictionary<string, Type> _typeCache = new ConcurrentDictionary<string, Type>();

        public DefaultEnvelopeTypeLoader(IOptions<MeceqsTransportOptions> options)
        {
            _options = options.Value;
        }

        public Type LoadEnvelopeType(string messageType)
        {
            if (string.IsNullOrWhiteSpace(messageType))
                return null;

            // Caching the types in a dictionary is much faster than loading the type each time.
            // This way, only the first attempt to load a certain message type has a performance hit.
            // See Meceqs.Tests.Performance for benchmark tests

            return _typeCache.GetOrAdd(messageType, x =>
            {
                Type typeOfMessage = _options.ContractAssemblies
                    .SelectMany(a => a.ExportedTypes)
                    .FirstOrDefault(t => string.Equals(t.FullName, messageType, StringComparison.OrdinalIgnoreCase));

                if (typeOfMessage == null)
                {
                    throw new InvalidOperationException($"Type for message '{messageType}' could not be loaded.");
                }

                return typeof(Envelope<>).MakeGenericType(typeOfMessage);
            });
        }
    }
}