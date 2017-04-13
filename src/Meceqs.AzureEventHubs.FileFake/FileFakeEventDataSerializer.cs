using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Meceqs.Transport;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Meceqs.AzureEventHubs.FileFake
{
    public static class FileFakeEventDataSerializer
    {
        private static readonly PropertyInfo SystemPropertiesSetter = typeof(EventData).GetProperty("SystemProperties");
        private static readonly ConstructorInfo SystemPropertiesCtor = typeof(EventData.SystemPropertiesCollection).GetTypeInfo().DeclaredConstructors.First();
        private static readonly PropertyInfo SystemPropertyEnqueuedTimeUtc = typeof(EventData.SystemPropertiesCollection).GetProperty("EnqueuedTimeUtc");
        private static readonly PropertyInfo SystemPropertyOffset = typeof(EventData.SystemPropertiesCollection).GetProperty("Offset");
        private static readonly PropertyInfo SystemPropertySequenceNumber = typeof(EventData.SystemPropertiesCollection).GetProperty("SequenceNumber");

        public static string Serialize(EventData eventData)
        {
            string serializedEnvelope = Encoding.UTF8.GetString(eventData.Body.Array);

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                foreach (var kvp in eventData.Properties)
                {
                    writer.WritePropertyName(kvp.Key);
                    writer.WriteValue(kvp.Value);
                }

                writer.WritePropertyName(nameof(eventData.SystemProperties.EnqueuedTimeUtc));
                writer.WriteValue(DateTime.UtcNow);

                writer.WritePropertyName("Body");
                writer.WriteValue(serializedEnvelope);

                writer.WriteEndObject();
            }

            return sb.ToString();
        }

        public static EventData Deserialize(string serializedEventData, long sequenceNumber)
        {
            JObject jsonEventData = JObject.Parse(serializedEventData);

            string serializedEnvelope = jsonEventData.GetValue("Body").ToString();
            var eventData = new EventData(Encoding.UTF8.GetBytes(serializedEnvelope));

            foreach (var header in TransportHeaderNames.AsList())
            {
                string value = jsonEventData.GetValue(header)?.ToString();
                eventData.Properties[header] = value;
            }

            // System properties are internal - that's why we need reflection :(

            var systemProperties = (EventData.SystemPropertiesCollection)SystemPropertiesCtor.Invoke(new object[] { });

            SystemPropertySequenceNumber.SetValue(systemProperties, sequenceNumber);
            SystemPropertyOffset.SetValue(systemProperties, sequenceNumber.ToString());

            var enqueuedTimeUtc = (DateTime)jsonEventData.GetValue(nameof(systemProperties.EnqueuedTimeUtc)).ToObject(typeof(DateTime));
            SystemPropertyEnqueuedTimeUtc.SetValue(systemProperties, enqueuedTimeUtc);

            SystemPropertiesSetter.SetValue(eventData, systemProperties);

            return eventData;
        }
    }
}