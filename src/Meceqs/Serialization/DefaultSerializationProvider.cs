using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Options;

namespace Meceqs.Serialization
{
    public class DefaultSerializationProvider : ISerializationProvider
    {
        private readonly IReadOnlyList<ISerializer> _serializers;

        public IReadOnlyList<string> SupportedContentTypes { get; }

        public DefaultSerializationProvider(IOptions<SerializationOptions> options)
        {
            Guard.NotNull(options?.Value, nameof(options));

            _serializers = options.Value.Serializers;

            if (_serializers.Count == 0)
            {
                throw new InvalidOperationException("No serializers have been configured.");
            }

            SupportedContentTypes = GetSupportedContentTypes(_serializers);
        }

        public ISerializer GetSerializer(Type objectType)
        {
            Guard.NotNull(objectType, nameof(objectType));

            foreach (var serializer in _serializers)
            {
                if (serializer.CanSerializeType(objectType))
                    return serializer;
            }

            throw new InvalidOperationException($"No serializer found that can handle the type '{objectType.FullName}'.");
        }

        public ISerializer GetSerializer(IEnumerable<string> supportedContentTypes)
        {
            Guard.NotNull(supportedContentTypes, nameof(supportedContentTypes));

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
                throw new InvalidOperationException($"No serializer matching content types '{string.Join(",", supportedContentTypes)}'.");
            }
            else
            {
                throw new ArgumentException("The list may not be empty", nameof(supportedContentTypes));
            }
        }

        public bool TryGetSerializer(string contentType, out ISerializer serializer)
        {
            Guard.NotNull(contentType, nameof(contentType));

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

        private static IReadOnlyList<string> GetSupportedContentTypes(IEnumerable<ISerializer> serializers)
        {
            var contentTypes = new List<string>();

            foreach (var serializer in serializers)
            {
                if (!contentTypes.Contains(serializer.ContentType))
                {
                    contentTypes.Add(serializer.ContentType);
                }
            }

            return contentTypes;
        }
    }
}
