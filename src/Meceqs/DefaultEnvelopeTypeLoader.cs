using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Meceqs
{
    public class DefaultEnvelopeTypeLoader : IEnvelopeTypeLoader
    {
        private readonly IList<Assembly> _contractAssemblies;

        private readonly ConcurrentDictionary<string, Type> _typeCache = new ConcurrentDictionary<string, Type>();

        public DefaultEnvelopeTypeLoader(IList<Assembly> contractAssemblies)
        {
            Check.NotNull(contractAssemblies, nameof(contractAssemblies));

            _contractAssemblies = contractAssemblies;
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
                Type typeOfMessage = _contractAssemblies
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