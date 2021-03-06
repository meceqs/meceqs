using System.Collections.Generic;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Represents a dictionary of arbitrary items that are passed from middleware to middleware.
    /// These items are not serialized in the envelope.
    /// </summary>
    public class MessageContextItems : Dictionary<object, object>
    {
        /// <summary>
        /// Adds all items from <paramref name="other"/> to this instance.
        /// </summary>
        public void Add(MessageContextItems other)
        {
            if (other == null || other.Count == 0)
                return;

            foreach (var kvp in other)
            {
                Set(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Writes the given key:value pair to the dictionary.
        /// Any existing value for the key will be overwritten.
        /// </summary>
        public void Set(object key, object value)
        {
            if (key != null)
            {
                this[key] = value;
            }
        }

        /// <summary>
        /// Gets the item with the given key and casts it to the given result type.
        /// Returns null if the key does not exist.
        /// </summary>
        public T Get<T>(object key)
        {
            if (key != null)
            {
                if (TryGetValue(key, out object value))
                {
                    return (T)value;
                }
            }

            return default(T);
        }
    }
}
