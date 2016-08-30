using System;

namespace Meceqs
{
    /// <summary>
    /// Represents one unit that processed a message.
    /// </summary>
    public class EnvelopeHistoryEntry
    {
        /// <summary>
        /// The name of the pipeline which was used to process the message.
        /// </summary>
        public string Pipeline { get; set; }

        /// <summary>
        /// The name of the machine which processed the message.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The name of the application which processed the message.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// The time at which the message has been processed by the application.
        /// </summary>
        public DateTimeOffset CreatedOnUtc { get; set; }

        /// <summary>
        /// Additional information that is specific to the application which processed the message.
        /// </summary>
        public EnvelopeProperties Properties { get; set; } = new EnvelopeProperties();
    }
}