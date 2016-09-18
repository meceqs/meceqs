using System;
using System.Collections.Generic;
using Meceqs.Internal;

namespace Meceqs
{
    public class EnvelopeProperties : Dictionary<string, object>
    {
        public EnvelopeProperties()
            : base(StringComparer.OrdinalIgnoreCase /* to support camelCase and PascalCase serializers */)
        {
        }

        public T Get<T>(string key)
        {
            Check.NotNullOrWhiteSpace(key, nameof(key));

            object value;
            if (TryGetValue(key, out value))
            {
                return ValueConverter.Instance.ConvertValue<T>(value);
            }

            return default(T);
        }

        public T GetRequired<T>(string key)
        {
            Check.NotNullOrWhiteSpace(key, nameof(key));

            object value;
            if (TryGetValue(key, out value))
            {
                return ValueConverter.Instance.ConvertValue<T>(value);
            }

            throw new ArgumentOutOfRangeException(nameof(key), key, $"No entry found for key '{key}'");
        }
    }
}