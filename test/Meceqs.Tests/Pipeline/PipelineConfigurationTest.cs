using System;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Pipeline
{
    public class PipelineConfigurationTest
    {
        [Fact]
        public void Adding_the_same_pipelineName_throws()
        {
            var services = new ServiceCollection()
                .AddOptions();

            services.AddMeceqs(builder =>
            {
                builder
                    .AddPipeline("pipeline", pipeline => { })
                    .AddPipeline("pipeline", pipeline => { });
            });

            var serviceProvider = services.BuildServiceProvider();

            Should.Throw<ArgumentException>(() => serviceProvider.GetRequiredService<IPipelineProvider>());
        }
    }
}