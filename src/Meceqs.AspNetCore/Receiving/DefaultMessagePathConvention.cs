using System;
using System.Collections.Generic;

namespace Meceqs.AspNetCore.Receiving
{
    public class DefaultMessagePathConvention : IMessagePathConvention
    {
        private static readonly List<string> _suffixesToRemove = new List<string>
        {
            "Command",
            "Event",
            "Message",
            "Query",
            "Request"
        };

        public string GetPathForMessage(Type messageType)
        {
            Guard.NotNull(messageType, nameof(messageType));

            string messageName = messageType.Name;

            foreach (var suffix in _suffixesToRemove)
            {
                if (messageName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                {
                    messageName = messageName.Substring(0, messageName.Length - suffix.Length);
                    break;
                }
            }

            return messageName;
        }
    }
}
