using System;
using System.Collections.Generic;

namespace Meceqs
{
    public class MessageHistoryEntry
    {
        public string Pipeline { get; set; }
        public string Host { get; set; }
        public string Endpoint { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }
}