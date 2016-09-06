using System.Collections.Generic;

namespace Meceqs.Transport
{
    public class TransportHeaderNames
    {
        public static readonly string ContentType = "ContentType";
        public static readonly string MessageId = "MessageId";
        public static readonly string MessageName = "MessageName";
        public static readonly string MessageType = "MessageType";

        public static IEnumerable<string> AsList()
        {
            yield return ContentType;
            yield return MessageId;
            yield return MessageName;
            yield return MessageType;
        }
    }
}