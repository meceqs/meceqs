using System;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Meceqs.Tests.Pipeline
{
    public class PipelineConfigurationTest
    {
        [Fact]
        public void Adding_the_same_pipelineName_fails()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddOptions()
                .AddMeceqs(builder =>
                {
                    builder
                        .AddPipeline("pipeline", pipeline =>
                        {
                            pipeline.Use((ctx, next) => { return next(); });
                        })
                        .AddPipeline("pipeline", pipeline =>
                        {
                            pipeline.Use((ctx, next) => { return next(); });
                        });
                })
                .BuildServiceProvider();

            Assert.Throws<InvalidOperationException>(() => serviceProvider.GetRequiredService<IPipelineProvider>());
        }
    }
}