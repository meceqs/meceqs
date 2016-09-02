using System.Threading.Tasks;
using Meceqs.Filters.TypedHandling;
using Microsoft.Extensions.Logging;

namespace CustomerContext.WebApi.Infrastructure
{
    public class SingletonHandleInterceptor : IHandleInterceptor
    {
        private readonly ILogger _logger;

        public SingletonHandleInterceptor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SingletonHandleInterceptor>();
        }

        public Task OnHandleExecuting(HandleContext context)
        {
            _logger.LogInformation("OnHandleExecuting for {MessageType}", context.Message.GetType());
            return Task.CompletedTask;
        }

        public Task OnHandleExecuted(HandleContext context)
        {
            _logger.LogInformation("OnHandleExecuted for {MessageType}", context.Message.GetType());
            return Task.CompletedTask;
        }
    }
}