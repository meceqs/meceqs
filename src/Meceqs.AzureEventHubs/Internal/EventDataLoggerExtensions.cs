using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Meceqs.AzureEventHubs.Internal;
using Microsoft.Azure.EventHubs;

namespace Microsoft.Extensions.Logging
{
    internal static class EventDataLoggerExtensions
    {
        private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

        public static IDisposable EventDataScope(this ILogger logger, EventData message)
        {
            return logger.BeginScope(new EventDataLogScope(message));
        }

        public static void ReceiveStarting(this ILogger logger, EventData message)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.Log(
                    logLevel: LogLevel.Information,
                    eventId: LoggerEventIds.ReceiveStarting,
                    state: new ReceiveStartingState(message),
                    exception: null,
                    formatter: ReceiveStartingState.Callback);
            }
        }

        public static void ReceiveFailed(this ILogger logger, EventData message, Exception ex)
        {
            logger.LogError(LoggerEventIds.ReceiveFailed, ex, "Receive failed with exception");
        }

        public static void ReceiveFinished(this ILogger logger, bool success, long startTimestamp, long currentTimestamp)
        {
            // Don't log if Information logging wasn't enabled at start or end of request as time will be wildly wrong.
            if (startTimestamp != 0)
            {
                var elapsed = new TimeSpan((long)(TimestampToTicks * (currentTimestamp - startTimestamp)));

                logger.Log(
                    logLevel: LogLevel.Information,
                    eventId: LoggerEventIds.ReceiveFinished,
                    state: new ReceiveFinishedState(success, elapsed),
                    exception: null,
                    formatter: ReceiveFinishedState.Callback);
            }
        }

        internal class EventDataLogScope : IReadOnlyList<KeyValuePair<string, object>>
        {
            private readonly EventData _eventData;

            private string _cachedToString;

            public EventDataLogScope(EventData eventData)
            {
                _eventData = eventData;
            }

            public int Count => 3;

            public KeyValuePair<string, object> this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return new KeyValuePair<string, object>(nameof(_eventData.SystemProperties.Offset), _eventData.SystemProperties.Offset);
                        case 1:
                            return new KeyValuePair<string, object>(nameof(_eventData.SystemProperties.SequenceNumber), _eventData.SystemProperties.SequenceNumber);
                        case 2:
                            return new KeyValuePair<string, object>(nameof(_eventData.SystemProperties.EnqueuedTimeUtc), _eventData.SystemProperties.EnqueuedTimeUtc);
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
                        "Offset:{0} Sequence:{1} Enqueued:{2}",
                        _eventData.SystemProperties.Offset,
                        _eventData.SystemProperties.SequenceNumber,
                        _eventData.SystemProperties.EnqueuedTimeUtc
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

        private class ReceiveStartingState : IReadOnlyList<KeyValuePair<string, object>>
        {
            internal static readonly Func<object, Exception, string> Callback = (state, exception) => ((ReceiveStartingState)state).ToString();

            private readonly EventData _eventData;

            private string _cachedToString;

            public ReceiveStartingState(EventData eventData)
            {
                _eventData = eventData;
            }

            public int Count => 3;

            public KeyValuePair<string, object> this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return new KeyValuePair<string, object>(nameof(_eventData.SystemProperties.Offset), _eventData.SystemProperties.Offset);
                        case 1:
                            return new KeyValuePair<string, object>(nameof(_eventData.SystemProperties.SequenceNumber), _eventData.SystemProperties.SequenceNumber);
                        case 2:
                            return new KeyValuePair<string, object>(nameof(_eventData.SystemProperties.EnqueuedTimeUtc), _eventData.SystemProperties.EnqueuedTimeUtc);
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
                        "Receive starting Offset:{0} Sequence:{1} Enqueued:{2}",
                        _eventData.SystemProperties.Offset,
                        _eventData.SystemProperties.SequenceNumber,
                        _eventData.SystemProperties.EnqueuedTimeUtc);
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

        private class ReceiveFinishedState : IReadOnlyList<KeyValuePair<string, object>>
        {
            internal static readonly Func<object, Exception, string> Callback = (state, exception) => ((ReceiveFinishedState)state).ToString();

            private readonly bool _success;
            private readonly TimeSpan _elapsed;

            private string _cachedToString;

            public int Count => 2;

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

            public ReceiveFinishedState(bool success, TimeSpan elapsed)
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
                        "Receive finished in {0}ms Success:{1}",
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