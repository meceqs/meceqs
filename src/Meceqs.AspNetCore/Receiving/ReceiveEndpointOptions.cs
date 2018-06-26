using System;
using System.Collections.Generic;

namespace Meceqs.AspNetCore.Receiving
{
    public class ReceiveEndpointOptions
    {
        private readonly List<string> _receivers = new List<string>();

        public IReadOnlyList<string> Receivers => _receivers;

        public void AddReceiver(string name)
        {
            Guard.NotNull(name, nameof(name));

            if (_receivers.Contains(name))
            {
                throw new InvalidOperationException($"Receiver '{name}' already exists.");
            }

            _receivers.Add(name);
        }
    }
}
