using System;
using System.Collections.Generic;

namespace Meceqs.HttpSender
{
    /// <summary>
    /// The default convention will assume that message endpoints do not use suffixes
    /// (e.g. "Message", "Command", ...) in their URI.
    /// </summary>
    public class DefaultEndpointMessageConvention : IEndpointMessageConvention
    {
        private static readonly List<string> _suffixesToRemove = new List<string>
        {
            "Command",
            "Event",
            "Message",
            "Query",
            "Request"
        };

        public string GetRelativePath(Type messageType)
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