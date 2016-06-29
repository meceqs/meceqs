using System;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus
{
    public class DefaultBrokeredMessageConverter : IBrokeredMessageConverter
    {
        public Envelope ConvertToEnvelope(BrokeredMessage brokeredMessage)
        {
            if (brokeredMessage == null)
                throw new ArgumentNullException(nameof(brokeredMessage));

            throw new NotImplementedException();
        }
    }
}