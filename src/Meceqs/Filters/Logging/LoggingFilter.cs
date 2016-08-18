using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.Extensions.Logging;

namespace Meceqs.Filters.Logging
{
    public class LoggingFilter
    {
        private readonly FilterDelegate _next;
        private readonly ILogger _logger;

        public LoggingFilter(FilterDelegate next, ILoggerFactory loggerFactory)
        {
            Check.NotNull(next, nameof(next));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _next = next;
            _logger = loggerFactory.CreateLogger<LoggingFilter>();
        }

        public Task Invoke(FilterContext context)
        {
            _logger.LogInformation(
                "Executing pipeline {pipeline} for {messageType}/{messageId}",
                context.PipelineName,
                context.MessageType,
                context.Envelope.MessageId);

            return _next(context);
        }
    }
}