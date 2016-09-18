using System;
using System.IO;

namespace Meceqs.Serialization
{
    public interface IResultDeserializer
    {
        object DeserializeResultFromStream(string contentType, Stream serializedResult, Type resultType);
    }
}