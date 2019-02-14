using System.Collections.Generic;

namespace Meceqs.Transport
{
    public class TransportHeaderNames
    {
        public const string ContentType = "Meceqs-ContentType";
        public const string MessageId = "Meceqs-MessageId";
        public const string CorrelationId = "Meceqs-CorrelationId";
        public const string MessageType = "Meceqs-MessageType";

        public const string HeaderPrefix = "Meceqs-Header-";

        public static IEnumerable<string> AsList()
        {
            yield return ContentType;
            yield return MessageId;
            yield return CorrelationId;
            yield return MessageType;
        }
    }
}
