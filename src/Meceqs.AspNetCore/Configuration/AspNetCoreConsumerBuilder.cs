using Meceqs.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.AspNetCore.Configuration
{
    public class AspNetCoreConsumerBuilder : TransportConsumerBuilder<IAspNetCoreConsumerBuilder, AspNetCoreConsumerOptions>,
        IAspNetCoreConsumerBuilder
    {
        public override IAspNetCoreConsumerBuilder Instance => this;

        public AspNetCoreConsumerBuilder()
        {
            PipelineStartHook = pipeline =>
            {
                // Especially in dev environments, it's common that people use Fiddler etc.
                // To make sure, they don't have to add every field (message type can be implied etc.),
                // they are automatically added.
                pipeline.UseEnvelopeSanitizer();

                // This connects the pipeline with the ASP.NET Core request (RequestServices, User, ...).
                pipeline.UseAspNetCoreRequest();
            };
        }
    }
}