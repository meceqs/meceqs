using System;
using System.Collections.Generic;

namespace Meceqs.AspNetCore.Receiving
{
    public class DefaultMessagePathConvention : IMessagePathConvention
    {
        private static readonly List<string> _suffixesToRemove = new List<string>
        {
            "Message",
            "Command",
            "Event",
            "Query"
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