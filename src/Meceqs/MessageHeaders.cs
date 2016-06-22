using System.Collections.Generic;

namespace Meceqs
{
    public class MessageHeaders : Dictionary<string, string>
    {
        public void SetValue(string headerName, object value)
        {
            // Handlers have to be able to deal with missing/empty headers anyway,
            // so there's no point in adding them here.

            if (string.IsNullOrWhiteSpace(headerName) || value == null)
                return;

            this[headerName] = MessageValueConverter.ConvertToInvariantString(value);
        }
    }
}