using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Meceqs.Transport.AzureServiceBus.Internal;
using Microsoft.ServiceBus.Messaging;

namespace Microsoft.Extensions.Logging
{
    internal static class BrokeredMessageLoggerExtensions
    {
        private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

        public static IDisposable BrokeredMessageScope(this ILogger logger, BrokeredMessage message)
        {
            return logger.BeginScope(new BrokeredMessageLogScope(message));
        }

        public static void ConsumeStarting(this ILogger logger, BrokeredMessage message)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.Log(
                    logLevel: LogLevel.Information,
                    eventId: LoggerEventIds.ConsumeStarting,
                    state: new ConsumeStartingState(message),
                    exception: null,
                    formatter: ConsumeStartingState.Callback);
            }
        }

        public static void ConsumeFailed(this ILogger logger, BrokeredMessage message, Exception ex)
        {
            logger.LogError(LoggerEventIds.ConsumeFailed, ex, "Consume failed with exception");
        }

        public static void ConsumeFinished(this ILogger logger, bool success, long startTimestamp, long currentTimestamp)
        {
            // Don't log if Information logging wasn't enabled at start or end of request as time will be wildly wrong.
            if (startTimestamp != 0)
            {
                var elapsed = new TimeSpan((long)(TimestampToTicks * (currentTimestamp - startTimestamp)));

                logger.Log(
                    logLevel: LogLevel.Information,
                    eventId: LoggerEventIds.ConsumeFinished,
                    state: new ConsumeFinishedState(success, elapsed),
                    exception: null,
                    formatter: ConsumeFinishedState.Callback);
            }
        }

        internal class BrokeredMessageLogScope : IReadOnlyList<KeyValuePair<string, object>>
        {
            private readonly BrokeredMessage _message;

            private string _cachedToString;

            public BrokeredMessageLogScope(BrokeredMessage message)
            {
                _message = message;
            }

            public int Count => 2;

            public KeyValuePair<string, object> this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return new KeyValuePair<string, object>("MessageId", _message.MessageId);
                        case 1:
                            return new KeyValuePair<string, object>("ContentType", _message.ContentType);
                        default:
                            throw new IndexOutOfRangeException(nameof(index));
                    }
                }
            }

            public override string ToString()
            {
                if (_cachedToString == null)
                {
                    _cachedToString = string.Format(
                        CultureInfo.InvariantCulture,
                        "ID:{0} ContentType:{1}",
                        _message.MessageId,
                        _message.ContentType
                    );
                }

                return _cachedToString;
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class ConsumeStartingState : IReadOnlyList<KeyValuePair<string, object>>
        {
            internal static readonly Func<object, Exception, string> Callback = (state, exception) => ((ConsumeStartingState)state).ToString();

            private readonly BrokeredMessage _message;

            private string _cachedToString;

            public ConsumeStartingState(BrokeredMessage message)
            {
                _message = message;
            }

            public int Count => 5;

            public KeyValuePair<string, object> this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return new KeyValuePair<string, object>("SequenceNumber", _message.SequenceNumber);
                        case 1:
                            return new KeyValuePair<string, object>("DeliveryCount", _message.DeliveryCount);
                        case 2:
                            return new KeyValuePair<string, object>("EnqueuedTimeUtc", _message.EnqueuedTimeUtc);
                        case 3:
                            return new KeyValuePair<string, object>("CorrelationId", _message.CorrelationId);
                        case 4:
                            return new KeyValuePair<string, object>("ExpiresAtUtc", _message.ExpiresAtUtc);
                        default:
                            throw new IndexOutOfRangeException(nameof(index));
                    }
                }
            }

            public override string ToString()
            {
                if (_cachedToString == null)
                {
                    _cachedToString = string.Format(
                        CultureInfo.InvariantCulture,
                        "Consume starting SequenceNr:{0} DeliveryCount:{1} Enqueued:{2}",
                        _message.SequenceNumber,
                        _message.DeliveryCount,
                        _message.EnqueuedSequenceNumber);
                }

                return _cachedToString;
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                for (int i = 0; i < Count; ++i)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class ConsumeFinishedState : IReadOnlyList<KeyValuePair<string, object>>
        {
            internal static readonly Func<object, Exception, string> Callback = (state, exception) => ((ConsumeFinishedState)state).ToString();

            private readonly bool _success;
            private readonly TimeSpan _elapsed;

            private string _cachedToString;

            public int Count
            {
                get
                {
                    return 2;
                }
            }

            public KeyValuePair<string, object> this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return new KeyValuePair<string, object>("ElapsedMilliseconds", _elapsed.TotalMilliseconds);
                        case 1:
                            return new KeyValuePair<string, object>("Success", _success);
                        default:
                            throw new IndexOutOfRangeException(nameof(index));
                    }
                }
            }

            public ConsumeFinishedState(bool success, TimeSpan elapsed)
            {
                _success = success;
                _elapsed = elapsed;
            }

            public override string ToString()
            {
                if (_cachedToString == null)
                {
                    _cachedToString = string.Format(
                        CultureInfo.InvariantCulture,
                        "Consume finished in {0}ms Success:{1}",
                        _elapsed.TotalMilliseconds,
                        _success);
                }

                return _cachedToString;
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                for (int i = 0; i < Count; ++i)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}