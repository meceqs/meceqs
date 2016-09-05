using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.Extensions.Logging;

namespace Meceqs.Filters.MessageIgnorer
{
    public class MessageIgnorerFilter
    {
        private readonly ILogger _logger;

        public MessageIgnorerFilter(FilterDelegate next, ILoggerFactory loggerFactory)
        {
            // "next" is not stored because this is a terminating filter.

            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<MessageIgnorerFilter>();
        }

        public Task Invoke(FilterContext context)
        {
            _logger.LogDebug(
                "Ignoring message {MessageType}/{MessageId}",
                context.Envelope.MessageType,
                context.Envelope.MessageId);

            return Task.CompletedTask;
        }
    }

}