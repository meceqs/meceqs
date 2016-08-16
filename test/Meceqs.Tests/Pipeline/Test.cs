using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Tests.Pipeline
{
    public class Test
    {
        public class ValidationFilter
        {
            private readonly FilterDelegate _next;

            public ValidationFilter(FilterDelegate next)
            {
                _next = next;
            }

            public Task Invoke(FilterContext context)
            {
                //context.Message;

                return _next(context);
            }
        }

        private IPipelineBuilder GetPipelineBuilder(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var pipelineBuilder = new DefaultPipelineBuilder(serviceProvider, "name");
            
            return pipelineBuilder;
        }

        public void ConfigurePipeline()
        {
            var services = new ServiceCollection();
            var builder = GetPipelineBuilder(services);

            // Filter type
            builder.UseFilter<ValidationFilter>();

            // Filter specific extension method
            //builder.UseValidation();

            // In-line filter
            builder.Use((ctx, next) => 
            {
                // ctx.Message

                return next();
            });
        }
    }
}