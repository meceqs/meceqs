using System;
using System.Collections.Generic;

namespace Meceqs.Serialization
{
    public interface ISerializationProvider
    {
        IReadOnlyList<string> GetSupportedContentTypes(Type objectType = null);

        ISerializer GetSerializer(Type objectType);

        ISerializer GetSerializer(Type objectType, string supportedContentType);

        ISerializer GetSerializer(Type objectType, IEnumerable<string> supportedContentTypes);

        bool TryGetSerializer(Type objectType, out ISerializer serializer);

        bool TryGetSerializer(Type objectType, string supportedContentType, out ISerializer serializer);

        bool TryGetSerializer(Type objectType, IEnumerable<string> supportedContentTypes, out ISerializer serializer);
    }
}
