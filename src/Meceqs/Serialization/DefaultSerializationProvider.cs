using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Meceqs.Serialization
{
    public class DefaultSerializationProvider : ISerializationProvider
    {
        private readonly List<ISerializer> _serializers;
        private readonly IEnvelopeTypeLoader _envelopeTypeLoader;

        public DefaultSerializationProvider(IEnumerable<ISerializer> serializers, IEnvelopeTypeLoader envelopeTypeLoader)
        {
            _serializers = serializers.ToList();
            _envelopeTypeLoader = envelopeTypeLoader;
        }

        public ISerializer GetDefaultSerializer()
        {
            if (_serializers.Count > 0)
            {
                return _serializers[0];
            }

            throw new InvalidOperationException($"No services of type '{nameof(ISerializer)}' have been registered.");
        }

        public ISerializer GetSerializer(IEnumerable<string> supportedContentTypes)
        {
            if (supportedContentTypes != null)
            {
                bool hasItems = false;
                foreach (var supportedContentType in supportedContentTypes)
                {
                    hasItems = true;
                    if (TryGetSerializer(supportedContentType, out ISerializer serializer))
                    {
                        return serializer;
                    }
                }

                if (hasItems)
                {
                    throw new InvalidOperationException($"No serializer matching content types '{string.Join(",", supportedContentTypes)}'");
                }
            }

            return GetDefaultSerializer();
        }

        public bool TryGetSerializer(string contentType, out ISerializer serializer)
        {
            serializer = null;

            foreach (var supportedSerializer in _serializers)
            {
                if (supportedSerializer.ContentType == contentType)
                {
                    serializer = supportedSerializer;
                    return true;
                }
            }

            return false;
        }

        public object Deserialize(string contentType, Type objectType, Stream serializedObject)
        {
            Guard.NotNull(contentType, nameof(contentType));
            Guard.NotNull(objectType, nameof(objectType));
            Guard.NotNull(serializedObject, nameof(serializedObject));

            if (!TryGetSerializer(contentType, out var serializer))
            {
                throw new NotSupportedException($"ContentType '{contentType}' is not supported.");
            }

            return serializer.Deserialize(objectType, serializedObject);
        }

        public Envelope DeserializeEnvelope(string contentType, byte[] serializedEnvelope, string messageType)
        {
            Guard.NotNull(contentType, nameof(contentType));
            Guard.NotNull(messageType, nameof(messageType));

            if (!TryGetSerializer(contentType, out var serializer))
            {
                throw new NotSupportedException($"ContentType '{contentType}' is not supported.");
            }

            Type envelopeType = _envelopeTypeLoader.LoadEnvelopeType(messageType);

            return (Envelope)serializer.Deserialize(envelopeType, serializedEnvelope);
        }

        public Envelope DeserializeEnvelope(string contentType, Stream serializedEnvelope, string messageType)
        {
            Guard.NotNull(contentType, nameof(contentType));
            Guard.NotNull(messageType, nameof(messageType));

            if (!TryGetSerializer(contentType, out var serializer))
            {
                throw new NotSupportedException($"ContentType '{contentType}' is not supported.");
            }

            Type envelopeType = _envelopeTypeLoader.LoadEnvelopeType(messageType);

            return (Envelope)serializer.Deserialize(envelopeType, serializedEnvelope);
        }
    }
}
