using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Meceqs.Serialization
{
    public class DefaultSerializationProvider : ISerializationProvider
    {
        private static readonly IEnumerable<string> EmptyContentTypes = new List<string>().AsReadOnly();

        private readonly IReadOnlyList<ISerializer> _serializers;
        private readonly IReadOnlyList<string> _supportedContentTypes;

        public DefaultSerializationProvider(IOptions<SerializationOptions> options)
        {
            Guard.NotNull(options?.Value, nameof(options));

            _serializers = options.Value.Serializers;

            if (_serializers.Count == 0)
            {
                throw new InvalidOperationException("No serializers have been configured.");
            }

            _supportedContentTypes = GetSupportedContentTypes(_serializers);
        }

        public IReadOnlyList<string> GetSupportedContentTypes(Type objectType = null)
        {
            if (objectType == null)
            {
                return _supportedContentTypes;
            }

            List<string> supportedContentTypes = new List<string>();
            foreach (var serializer in _serializers)
            {
                if (!supportedContentTypes.Contains(serializer.ContentType) && serializer.CanSerializeType(objectType))
                {
                    supportedContentTypes.Add(serializer.ContentType);
                }
            }

            return supportedContentTypes;
        }

        public ISerializer GetSerializer(Type objectType)
        {
            return GetSerializer(objectType, EmptyContentTypes);
        }

        public ISerializer GetSerializer(Type objectType, string supportedContentType)
        {
            Guard.NotNullOrWhiteSpace(supportedContentType, nameof(supportedContentType));

            return GetSerializer(objectType, new List<string> { supportedContentType });
        }

        public ISerializer GetSerializer(Type objectType, IEnumerable<string> supportedContentTypes)
        {
            if (!TryGetSerializer(objectType, supportedContentTypes, out var serializer))
            {
                throw new InvalidOperationException(
                    $"No serializer matches object type '{objectType.FullName}' " +
                    $"and content types '{string.Join(",", supportedContentTypes)}'.");
            }

            return serializer;
        }

        public bool TryGetSerializer(Type objectType, out ISerializer serializer)
        {
            return TryGetSerializer(objectType, EmptyContentTypes, out serializer);
        }

        public bool TryGetSerializer(Type objectType, string supportedContentType, out ISerializer serializer)
        {
            Guard.NotNullOrWhiteSpace(supportedContentType, nameof(supportedContentType));

            return TryGetSerializer(objectType, new List<string> { supportedContentType }, out serializer);
        }

        public bool TryGetSerializer(Type objectType, IEnumerable<string> supportedContentTypes, out ISerializer serializer)
        {
            Guard.NotNull(objectType, nameof(objectType));
            Guard.NotNull(supportedContentTypes, nameof(supportedContentTypes));

            foreach (var existingSerializer in _serializers)
            {
                if (existingSerializer.CanSerializeType(objectType))
                {
                    if (!supportedContentTypes.Any())
                    {
                        // The client doesn't have any preference regarding the content type
                        // so we can return the "best" (= the first that matches)
                        serializer = existingSerializer;
                        return true;
                    }

                    if (supportedContentTypes.Any(x => string.Equals(x, existingSerializer.ContentType, StringComparison.OrdinalIgnoreCase)))
                    {
                        serializer = existingSerializer;
                        return true;
                    }
                }
            }

            serializer = null;
            return false;
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
