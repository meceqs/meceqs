using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.ServiceBus.Messaging;

namespace Meceqs.AzureServiceBus
{
    public class DefaultBrokeredMessageConverter : IBrokeredMessageConverter
    {
        private static MethodInfo _getBodyMethodInfo = typeof(BrokeredMessage).GetMethod(nameof(BrokeredMessage.GetBody));

        // Caches the delegates for every type to increase performance
        private readonly ConcurrentDictionary<string, Func<BrokeredMessage, Envelope>> _getBodyMethodCache = new ConcurrentDictionary<string, Func<BrokeredMessage, Envelope>>();

        public Envelope ConvertToEnvelope(BrokeredMessage brokeredMessage)
        {
            if (brokeredMessage == null)
                throw new ArgumentNullException(nameof(brokeredMessage));

            // TODO @cweiss validate ContentType?

            // BrokeredMessage.GetBody<T> is a generic method. It can not be called directly because T is not known
            // at compile-time. Instead, T is defined by a value in the metadata of BrokeredMessage.
            // This requires us to call the method with Reflection. However, creating the typed MethodInfo-object
            // is very expensive. For this reason, we create a delegate for the MethodInfo-object and cache it
            // for each message type. This way, most of the reflection overhead only affects the first message.
            // 
            // This requires this class to be a singleton!

            Func<BrokeredMessage, Envelope> typedGetBodyMethodFunc = _getBodyMethodCache.GetOrAdd(brokeredMessage.ContentType, (messageTypeName) =>
            {
                // there's no delegate in the cache for the requested message type -> create a new one

                Type messageType = Type.GetType(brokeredMessage.ContentType, throwOnError: true);
                MethodInfo typedGetBodyMethod = _getBodyMethodInfo.MakeGenericMethod(messageType);

                return (Func<BrokeredMessage, Envelope>)Delegate.CreateDelegate(
                    typeof(Func<BrokeredMessage, Envelope>), null, typedGetBodyMethod, throwOnBindFailure: true);
            });

            Envelope envelope = typedGetBodyMethodFunc(brokeredMessage);
            return envelope;
        }
    }
}