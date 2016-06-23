using System;
using System.Collections.Generic;

namespace Meceqs
{
    public class MessageHeaders : Dictionary<string, string>
    {
        public T GetValue<T>(string headerName)
        {
            if (string.IsNullOrWhiteSpace(headerName))
                throw new ArgumentNullException(nameof(headerName));

            string value;
            if (TryGetValue(headerName, out value))
            {
                return MessageValueConverter.FromInvariantString<T>(value);
            }

            return default(T);
        }

        public void SetValue(string headerName, object value)
        {
            // Handlers have to be able to deal with missing/empty headers anyway,
            // so there's no point in adding them here.

            if (string.IsNullOrWhiteSpace(headerName) || value == null)
                return;

            this[headerName] = MessageValueConverter.ToInvariantString(value);
        }
    }
}