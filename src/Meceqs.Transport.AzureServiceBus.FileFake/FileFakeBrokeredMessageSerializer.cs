using System.IO;
using System.Text;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Meceqs.Transport.AzureServiceBus.FileFake
{
    public static class FileFakeBrokeredMessageSerializer
    {
        public static string Serialize(BrokeredMessage message)
        {
            string serializedEnvelope;
            using (StreamReader reader = new StreamReader(message.GetBody<Stream>(), Encoding.UTF8))
            {
                serializedEnvelope = reader.ReadToEnd();
            }

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                foreach (var kvp in message.Properties)
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

        public static BrokeredMessage Deserialize(string serializedBrokeredMessage)
        {
            JObject jsonMessage = JObject.Parse(serializedBrokeredMessage);

            string serializedEnvelope = jsonMessage.GetValue("Body").ToString();
            MemoryStream payloadStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedEnvelope));
            BrokeredMessage brokeredMessage = new BrokeredMessage(payloadStream, ownsStream: true);

            foreach (var header in TransportHeaderNames.AsList())
            {
                string value = jsonMessage.GetValue(header)?.ToString();
                brokeredMessage.Properties[header] = value;
            }

            brokeredMessage.ContentType = brokeredMessage.Properties[TransportHeaderNames.ContentType].ToString();
            brokeredMessage.MessageId = brokeredMessage.Properties[TransportHeaderNames.MessageId].ToString();

            brokeredMessage.CorrelationId = jsonMessage.GetValue(nameof(brokeredMessage.CorrelationId)).ToString();

            // Receiver properties are internal - that's why we need reflection :(
            // (if these properties are not set, accessing them will throw an exception.)

            SetPropertyValue(brokeredMessage, nameof(brokeredMessage.DeliveryCount), 1);
            SetPropertyValue(brokeredMessage, nameof(brokeredMessage.SequenceNumber), 1);
            SetPropertyValue(brokeredMessage, nameof(brokeredMessage.EnqueuedSequenceNumber), 1);

            return brokeredMessage;
        }

        private static void SetPropertyValue(BrokeredMessage message, string propertyName, object value)
        {
            typeof(BrokeredMessage).GetProperty(propertyName).SetValue(message, value);
        }
    }
}