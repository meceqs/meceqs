using System;

namespace Meceqs.Transport.AzureEventHubs
{
    public class EventHubConnection
    {
        public string ConnectionString { get; }

        public string EventHubName { get; }

        public EventHubConnection(string connectionInformation)
        {
            Check.NotNullOrWhiteSpace(connectionInformation, nameof(connectionInformation));

            string[] parts = connectionInformation.Split('|');

            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid eventHub connection string.", nameof(connectionInformation));
            }

            EventHubName = parts[0];
            ConnectionString = parts[1];

            if (string.IsNullOrEmpty(EventHubName))
            {
                throw new ArgumentException("Invalid eventHub connection string (EventHubName is empty)", nameof(connectionInformation));
            }

            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new ArgumentException("Invalid eventHub connection string (ConnectionString is empty)", nameof(connectionInformation));
            }
        }
    }
}