using System;
using System.IO;

namespace Meceqs.Serialization
{
    /// <summary>
    /// Allows serialization/deserialization of an object.
    /// </summary>
    public interface ISerializer
    {
        string ContentType { get; }

        bool CanSerializeType(Type objectType);

        byte[] SerializeToByteArray(object obj);

        void SerializeToStream(object obj, Stream stream);

        object Deserialize(Type objectType, byte[] serializedObject);

        object Deserialize(Type objectType, Stream serializedObject);
    }
}
