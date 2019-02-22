using System;
using System.IO;
using System.Text;
using Meceqs.Transport;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Meceqs.AzureEventHubs.FileFake
{
    public static class FileFakeEventDataSerializer
    {
        // From https://github.com/Azure/azure-event-hubs-dotnet/blob/dev/src/Microsoft.Azure.EventHubs/Primitives/ClientConstants.cs
        private const string EnqueuedTimeUtcName = "x-opt-enqueued-time";

        public static string Serialize(EventData eventData, string partitionKey)
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

                writer.WritePropertyName("PartitionKey");
                writer.WriteValue(partitionKey);

                writer.WritePropertyName(EnqueuedTimeUtcName);
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

            eventData.SystemProperties = new EventData.SystemPropertiesCollection(
                sequenceNumber: sequenceNumber,
                enqueuedTimeUtc: (DateTime)jsonEventData.GetValue(EnqueuedTimeUtcName).ToObject(typeof(DateTime)),
                offset: sequenceNumber.ToString(),
                partitionKey: jsonEventData.GetValue("PartitionKey").ToString());

            return eventData;
        }
    }
}
