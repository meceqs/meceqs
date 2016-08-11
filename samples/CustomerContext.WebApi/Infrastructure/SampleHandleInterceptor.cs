using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CustomerContext.Core;
using Meceqs.Filters.TypedHandling;
using Microsoft.Extensions.Logging;

namespace CustomerContext.WebApi.Infrastructure
{
    public class SampleHandleInterceptor : IHandleInterceptor
    {
        private readonly ILogger _logger;

        public SampleHandleInterceptor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SampleHandleInterceptor>();
        }

        public Task OnHandleExecuting(HandleContext context)
        {
            _logger.LogInformation("OnHandleExecuting for {MessageType}", context.Message.GetType());

            var customAttribute = context.HandleMethod.CustomAttributes
                .FirstOrDefault(x => x.AttributeType == typeof(CustomLogicAttribute));

            if (customAttribute != null)
            {
                _logger.LogWarning("Custom attribute found on method");
            }

            customAttribute = context.HandlerType.GetTypeInfo().CustomAttributes
                .FirstOrDefault(x => x.AttributeType == typeof(CustomLogicAttribute));

            if (customAttribute != null)
            {
                _logger.LogWarning("Custom attribute found on class");
            }

            return Task.CompletedTask;
        }

        public Task OnHandleExecuted(HandleContext context)
        {
            _logger.LogInformation("OnHandleExecuted for {MessageType}", context.Message.GetType());
            return Task.CompletedTask;
        }
    }
}