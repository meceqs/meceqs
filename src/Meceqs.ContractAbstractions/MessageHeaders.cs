using System;
using System.Collections.Generic;
using System.Globalization;

namespace Meceqs
{
    public class MessageHeaders : Dictionary<string, string>
    {
        public void SetValue(string headerName, object value)
        {
            // Consumers have to be able to deal with missing/empty headers anyway,
            // so there's no point in adding them here.

            if (string.IsNullOrWhiteSpace(headerName) || value == null)
                return;

            this[headerName] = MessageValueConverter.ConvertToInvariantString(value);
        }


    }

    public static class MessageValueConverter
    {
        public static string DateTimeFormat = "yyyy-MM-ddThh:mm:ss";
        public static string ConvertToInvariantString(object value)
        {
            if (value == null)
                return null;

// We want to use a fixed pattern for DateTime
            DateTime? dt = value as DateTime?;
            if (dt.HasValue)
            {
                return dt.Value.ToString(DateTimeFormat);
            }

            return (string) Convert.ChangeType(value, typeof(string), CultureInfo.InvariantCulture);
        }

    }
}