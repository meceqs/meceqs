using System;
using System.Globalization;

namespace Meceqs.Internal
{
    public class ValueConverter
    {
        public static ValueConverter Instance { get; set; } = new ValueConverter();

        public virtual T ConvertValue<T>(object value)
        {
            if (value == null)
                return default(T);

            Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            Type valueType = value.GetType();

            if (targetType == valueType)
                return (T)value;

            string strValue = value as string;
            if (strValue != null)
            {
                // Necessary parsing in case the dictionary was deserialized from a string.
                // The serializer should be smart enough to convert numeric values to their actual type,
                // that's why we only handle special scenarios here.

                if (targetType == typeof(DateTimeOffset))
                {
                    var dateValue = DateTimeOffset.Parse(strValue, CultureInfo.InvariantCulture).ToUniversalTime();
                    return (T)Convert.ChangeType(dateValue, targetType);
                }

                if (targetType == typeof(DateTime))
                {
                    var dateValue = DateTime.Parse(strValue, CultureInfo.InvariantCulture).ToUniversalTime();
                    return (T)Convert.ChangeType(dateValue, targetType);
                };
            }

            // the value is not a special case, so we just try a regular convert.
            return (T)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
    }
}