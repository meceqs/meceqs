using System;
using System.Collections.Generic;

namespace Meceqs.Pipeline
{
    public class FilterContextItems : Dictionary<string, object>
    {
        public void Set(string key, object value)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                this[key] = value;
            }
        }

        public T Get<T>(string key)
        {
            object value;
            if (key != null && TryGetValue(key, out value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            return default(T);
        }
    }
}