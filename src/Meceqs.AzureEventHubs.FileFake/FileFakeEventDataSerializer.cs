using System;
using System.IO;
using System.Text;
using Meceqs.Transport;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Meceqs.AzureEventHubs.FileFake
{
    public static class FileFakeEventDataSerializer
    {
        public static string Serialize(EventData eventData)
        {
            string serializedEnvelope;
            using (StreamReader reader = new StreamReader(eventData.GetBodyStream(), Encoding.UTF8))
            {
                serializedEnvelope = reader.ReadToEnd();
            }

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

                writer.WritePropertyName(nameof(eventData.EnqueuedTimeUtc));
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

            // Receiver properties are internal - that's why we need reflection :(

            SetPropertyValue(eventData, nameof(eventData.SequenceNumber), sequenceNumber);
            SetPropertyValue(eventData, nameof(eventData.Offset), sequenceNumber.ToString());

            var enqueuedTimeUtc = (DateTime)jsonEventData.GetValue(nameof(eventData.EnqueuedTimeUtc)).ToObject(typeof(DateTime));
            SetPropertyValue(eventData, nameof(eventData.EnqueuedTimeUtc), enqueuedTimeUtc);

            return eventData;
        }

        private static void SetPropertyValue(EventData eventData, string propertyName, object value)
        {
            typeof(EventData).GetProperty(propertyName).SetValue(eventData, value);
        }
    }
}