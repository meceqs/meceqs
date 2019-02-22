using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Meceqs.Serialization
{
    /// <summary>
    /// Represents a collection of serializers.
    /// </summary>
    public class SerializerCollection : Collection<ISerializer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerCollection"/> class that is empty.
        /// </summary>
        public SerializerCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerCollection"/> class
        /// as a wrapper for the specified list.
        /// </summary>
        /// <param name="list">The list that is wrapped by the new collection.</param>
        public SerializerCollection(IList<ISerializer> list)
            : base(list)
        {
        }

        /// <summary>
        /// Removes all serializers of the specified type.
        /// </summary>
        /// <typeparam name="T">The type to remove.</typeparam>
        public void RemoveType<T>() where T : ISerializer
        {
            RemoveType(typeof(T));
        }

        /// <summary>
        /// Removes all serializers of the specified type.
        /// </summary>
        /// <param name="formatterType">The type to remove.</param>
        public void RemoveType(Type formatterType)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                var formatter = this[i];
                if (formatter.GetType() == formatterType)
                {
                    RemoveAt(i);
                }
            }
        }
    }
}