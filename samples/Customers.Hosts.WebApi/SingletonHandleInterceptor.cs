using System.Threading.Tasks;
using Meceqs.TypedHandling;
using Microsoft.Extensions.Logging;

namespace Customers.Hosts.WebApi
{
    public class SingletonHandleInterceptor : IHandleInterceptor
    {
        private readonly ILogger _logger;

        public SingletonHandleInterceptor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SingletonHandleInterceptor>();
        }

        public async Task OnHandleExecutionAsync(HandleContext context, HandleExecutionDelegate next)
        {
            _logger.LogInformation("OnHandleExecuting for {MessageType}", context.Message.GetType());

            await next(context);

            _logger.LogInformation("OnHandleExecuted for {MessageType}", context.Message.GetType());
        }
    }
}