using System.Collections.Generic;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Represents a dictionary of arbitrary items that are passed from filter to filter.
    /// These items are not serialized in the envelope.
    /// </summary>
    public class FilterContextItems : Dictionary<object, object>
    {
        /// <summary>
        /// Adds all items from <paramref name="other"/> to this instance.
        /// </summary>
        public void Add(FilterContextItems other)
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
        /// Returns null ff the key does not exist.
        /// </summary>
        public T Get<T>(object key)
        {
            if (key != null)
            {
                object value;
                if (TryGetValue(key, out value))
                {
                    return (T)value;
                }
            }

            return default(T);
        }
    }
}