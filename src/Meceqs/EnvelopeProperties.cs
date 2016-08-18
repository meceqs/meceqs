using System;
using System.Collections.Generic;
using System.Globalization;

namespace Meceqs
{
    public class EnvelopeProperties : Dictionary<string, object>
    {
        public EnvelopeProperties()
            : base(StringComparer.OrdinalIgnoreCase /* to support camelCase and PascalCase serializers */)
        {
        }

        public T Get<T>(string headerName)
        {
            if (string.IsNullOrWhiteSpace(headerName))
                throw new ArgumentNullException(nameof(headerName));

            object value;
            if (TryGetValue(headerName, out value))
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }

            return default(T);
        }

        public void Set(string headerName, object value)
        {
            // Handlers have to be able to deal with missing/empty headers anyway,
            // so there's no point in adding them here.

            if (string.IsNullOrWhiteSpace(headerName) || value == null)
                return;

            this[headerName] = value;
        }
    }
}