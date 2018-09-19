using System;
using System.IO;
using System.Reflection;
using System.Text;
using Meceqs.Transport;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Meceqs.AzureServiceBus.FileFake
{
    public static class FileFakeServiceBusMessageSerializer
    {
        private static readonly PropertyInfo SystemPropertyEnqueuedTimeUtc = typeof(Message.SystemPropertiesCollection).GetProperty("EnqueuedTimeUtc");
        private static readonly PropertyInfo SystemPropertyDeliveryCount = typeof(Message.SystemPropertiesCollection).GetProperty("DeliveryCount");
        private static readonly PropertyInfo SystemPropertySequenceNumber = typeof(Message.SystemPropertiesCollection).GetProperty("SequenceNumber");

        public static string Serialize(Message message)
        {
            string serializedEnvelope = Encoding.UTF8.GetString(message.Body);

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                foreach (var kvp in message.UserProperties)
                {
                    writer.WritePropertyName(kvp.Key);
                    writer.WriteValue(kvp.Value);
                }

                writer.WritePropertyName(nameof(message.MessageId));
                writer.WriteValue(message.MessageId);

                writer.WritePropertyName(nameof(message.CorrelationId));
                writer.WriteValue(message.CorrelationId);

                writer.WritePropertyName(nameof(message.SystemProperties.EnqueuedTimeUtc));
                writer.WriteValue(DateTime.UtcNow);

                writer.WritePropertyName("Body");
                writer.WriteValue(serializedEnvelope);

                writer.WriteEndObject();
            }

            return sb.ToString();
        }

        public static Message Deserialize(string serializedServiceBusMessage)
        {
            JObject jsonMessage = JObject.Parse(serializedServiceBusMessage);

            string serializedEnvelope = jsonMessage.GetValue("Body").ToString();
            Message message = new Message(Encoding.UTF8.GetBytes(serializedEnvelope));

            foreach (var header in TransportHeaderNames.AsList())
            {
                string value = jsonMessage.GetValue(header)?.ToString();
                message.UserProperties[header] = value;
            }

            message.ContentType = message.UserProperties[TransportHeaderNames.ContentType].ToString();
            message.MessageId = message.UserProperties[TransportHeaderNames.MessageId].ToString();

            message.CorrelationId = jsonMessage.GetValue(nameof(message.CorrelationId)).ToString();

            // System properties are internal - that's why we need reflection :(

            var systemProperties = message.SystemProperties;

            SystemPropertyDeliveryCount.SetValue(systemProperties, 1);
            SystemPropertySequenceNumber.SetValue(systemProperties, 1);

            var enqueuedTimeUtc = (DateTime)jsonMessage.GetValue(nameof(systemProperties.EnqueuedTimeUtc)).ToObject(typeof(DateTime));
            SystemPropertyEnqueuedTimeUtc.SetValue(systemProperties, enqueuedTimeUtc);

            return message;
        }

        private static void SetPropertyValue(Message message, string propertyName, object value)
        {
            typeof(Message).GetProperty(propertyName).SetValue(message, value);
        }
    }
}
