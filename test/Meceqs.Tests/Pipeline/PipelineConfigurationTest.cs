using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Pipeline
{
    public class PipelineConfigurationTest
    {
        [Fact]
        public void Configuring_the_same_pipelineName_succeeds()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddOptions()
                .AddMeceqs(builder =>
                {
                    builder
                        .ConfigurePipeline("pipeline", pipeline =>
                        {
                            pipeline.Use((ctx, next) => { return next(); });
                        })
                        .ConfigurePipeline("pipeline", pipeline =>
                        {
                            pipeline.Use((ctx, next) => { return next(); });
                        });
                })
                .BuildServiceProvider();

            var pipelineProvider = serviceProvider.GetRequiredService<IPipelineProvider>();

            pipelineProvider.GetPipeline("pipeline").ShouldNotBeNull();
        }
    }
}