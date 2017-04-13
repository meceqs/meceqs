using System.IO;
using System.Reflection;
using System.Text;
using Meceqs.Transport;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Meceqs.AzureServiceBus.FileFake
{
    public static class FileFakeBrokeredMessageSerializer
    {
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

            // Receiver properties are internal - that's why we need reflection :(
            // (if these properties are not set, accessing them will throw an exception.)

            SetPropertyValue(message, nameof(message.DeliveryCount), 1);
            SetPropertyValue(message, nameof(message.SequenceNumber), 1);
            SetPropertyValue(message, nameof(message.EnqueuedSequenceNumber), 1);

            return message;
        }

        private static void SetPropertyValue(Message message, string propertyName, object value)
        {
            typeof(Message).GetProperty(propertyName).SetValue(message, value);
        }
    }
}