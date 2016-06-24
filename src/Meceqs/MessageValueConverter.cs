using System;
using System.Globalization;

namespace Meceqs
{
    public static class MessageValueConverter
    {
        public static string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ"; // TODO @cweiss ISO 8601 - make this configurable

        public static T FromInvariantString<T>(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return default(T);

            Type type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            object result = null;

            // We want to use a fixed pattern for DateTime
            if (type == typeof(DateTime))
            {
                result = DateTime.ParseExact(value, DateTimeFormat, CultureInfo.InvariantCulture);
            }
            else
            {
                result = Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }

            return (T)result;
        }

        public static string ToInvariantString(object value)
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