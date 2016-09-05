using System.Collections.Generic;

namespace Meceqs.Pipeline
{
    public class FilterContextItems : Dictionary<object, object>
    {
        public void Add(FilterContextItems other)
        {
            if (other == null || other.Count == 0)
                return;

            foreach (var kvp in other)
            {
                Set(kvp.Key, kvp.Value);
            }
        }

        public void Set(object key, object value)
        {
            if (key != null)
            {
                this[key] = value;
            }
        }

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