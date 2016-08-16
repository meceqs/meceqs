using System;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PipelineMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddPipeline(this IMeceqsBuilder builder, string pipelineName, Action<IPipelineBuilder> setupAction)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));
            Check.NotNull(setupAction, nameof(setupAction));

            // Core Services
            builder.Services.TryAddSingleton<IFilterContextFactory, DefaultFilterContextFactory>();
            builder.Services.TryAddSingleton<IPipelineProvider, DefaultPipelineProvider>();
            builder.Services.TryAddTransient<IPipelineBuilder, DefaultPipelineBuilder>();

            builder.Services.Configure<PipelineOptions>(options => options.Pipelines.Add(pipelineName, setupAction));

            return builder;
        }
    }
}