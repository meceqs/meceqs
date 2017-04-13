using System.Reflection;
using System.Threading.Tasks;
using Customers.Core;
using Meceqs.TypedHandling;
using Microsoft.Extensions.Logging;

namespace Customers.Hosts.WebApi.Infrastructure
{
    public class SampleHandleInterceptor : IHandleInterceptor
    {
        private readonly ILogger _logger;

        public SampleHandleInterceptor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SampleHandleInterceptor>();
        }

        public async Task OnHandleExecutionAsync(HandleContext context, HandleExecutionDelegate next)
        {
            OnHandleExecuting(context);

            await next(context);

            OnHandleExecuted(context);
        }

        private void OnHandleExecuting(HandleContext context)
        {
            _logger.LogInformation("OnHandleExecuting for {MessageType}", context.Message.GetType());

            var customAttribute = context.HandleMethod.GetCustomAttribute(typeof(CustomLogicAttribute));

            if (customAttribute != null)
            {
                _logger.LogWarning("Custom attribute found on method");
            }

            customAttribute = context.HandlerType.GetTypeInfo().GetCustomAttribute(typeof(CustomLogicAttribute));

            if (customAttribute != null)
            {
                _logger.LogWarning("Custom attribute found on class");
            }
        }

        private void OnHandleExecuted(HandleContext context)
        {
            _logger.LogInformation("OnHandleExecuted for {MessageType}", context.Message.GetType());
        }
    }
}