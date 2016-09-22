using System;
using System.Reflection;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Pipeline;
using Meceqs.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeceqsBuilderExtensions
    {
        #region Pipelines

        // TODO @cweiss Rename to AddConsumePipeline/AddSendPipeline ? (it no longer adds the actual IMessageSender etc)

        public static IMeceqsBuilder AddConsumer(this IMeceqsBuilder builder, Action<IPipelineBuilder> pipeline)
        {
            return AddConsumer(builder, null, pipeline);
        }

        public static IMeceqsBuilder AddConsumer(this IMeceqsBuilder builder, string pipelineName, Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(pipeline, nameof(pipeline));

            builder.AddPipeline(pipelineName ?? MeceqsDefaults.ConsumePipelineName, pipeline);

            return builder;
        }

        public static IMeceqsBuilder AddSender(this IMeceqsBuilder builder, Action<IPipelineBuilder> pipeline)
        {
            return AddSender(builder, null, pipeline);
        }

        public static IMeceqsBuilder AddSender(this IMeceqsBuilder builder, string pipelineName, Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(pipeline, nameof(pipeline));

            builder.AddPipeline(pipelineName ?? MeceqsDefaults.SendPipelineName, pipeline);

            return builder;
        }

        public static IMeceqsBuilder AddPipeline(this IMeceqsBuilder builder, string pipelineName, Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));
            Check.NotNull(pipeline, nameof(pipeline));

            builder.Services.Configure<PipelineOptions>(options => options.Pipelines.Add(pipelineName, pipeline));

            return builder;
        }

        #endregion

        #region Serialization

        public static IMeceqsBuilder AddDeserializationAssembly<TType>(this IMeceqsBuilder builder)
        {
            var assembly = typeof(TType).GetTypeInfo().Assembly;

            return builder.AddDeserializationAssembly(assembly);
        }

        public static IMeceqsBuilder AddDeserializationAssembly(this IMeceqsBuilder builder, params Assembly[] assemblies)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(assemblies, nameof(assemblies));

            builder.Services.Configure<EnvelopeTypeLoaderOptions>(options => options.TryAddContractAssembly(assemblies));

            return builder;
        }

        #endregion
    }
}