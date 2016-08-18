using System;

namespace Meceqs
{
    public class EnvelopeHistoryEntry
    {
        public string Pipeline { get; set; }
        public string Host { get; set; }
        public string Endpoint { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public EnvelopeProperties Properties { get; set; } = new EnvelopeProperties();
    }
}