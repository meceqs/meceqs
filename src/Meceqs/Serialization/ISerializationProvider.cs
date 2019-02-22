﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Meceqs.Serialization
{
    public interface ISerializationProvider
    {
        IReadOnlyList<string> SupportedContentTypes { get; }

        bool TryGetSerializer(string contentType, out ISerializer serializer);

        ISerializer GetSerializer(Type objectType);

        ISerializer GetSerializer(IEnumerable<string> supportedContentTypes);

        object Deserialize(string contentType, Type objectType, Stream serializedObject);
    }
}
