using System;
using System.Globalization;

namespace Meceqs
{
    public static class MessageValueConverter
    {
        public static string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
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

            return (string)Convert.ChangeType(value, typeof(string), CultureInfo.InvariantCulture);
        }
    }
}