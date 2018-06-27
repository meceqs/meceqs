using System;
using System.Reflection;
using Meceqs;
using Meceqs.Pipeline;
using Meceqs.Serialization;
using Newtonsoft.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeceqsBuilderExtensions
    {
        /// <summary>
        /// Adds a named pipeline. This pipeline can be used by <see cref="Meceqs.Receiving.IMessageReceiver"/>
        /// and <see cref="Meceqs.Sending.IMessageSender"/>.
        /// </summary>
        public static IMeceqsBuilder AddPipeline(this IMeceqsBuilder builder, string pipelineName, Action<IPipelineBuilder> pipeline)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));
            Guard.NotNull(pipeline, nameof(pipeline));

            builder.Services.Configure<PipelineProviderOptions>(o => o.AddPipeline(pipelineName, pipeline));

            return builder;
        }

        /// <summary>
        /// Adds the default "Receive" pipeline. This pipeline will be used when <see cref="Meceqs.Receiving.IMessageReceiver"/>
        /// is used without specifying a named pipeline.
        /// </summary>
        public static IMeceqsBuilder AddReceivePipeline(this IMeceqsBuilder builder, Action<IPipelineBuilder> pipeline)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(pipeline, nameof(pipeline));

            builder.AddPipeline(MeceqsDefaults.ReceivePipelineName, pipeline);

            return builder;
        }

        /// <summary>
        /// Adds the default "Send" pipeline. This pipeline will be used when <see cref="Meceqs.Sending.IMessageSender"/>
        /// is used without specifying a named pipeline.
        /// </summary>
        public static IMeceqsBuilder AddSendPipeline(this IMeceqsBuilder builder, Action<IPipelineBuilder> pipeline)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(pipeline, nameof(pipeline));

            builder.AddPipeline(MeceqsDefaults.SendPipelineName, pipeline);

            return builder;
        }

        public static IMeceqsBuilder AddJsonSerialization(this IMeceqsBuilder builder, JsonSerializerSettings settings = null)
        {
            Guard.NotNull(builder, nameof(builder));

            var serializer = new Meceqs.Serialization.Json.JsonSerializer(settings);

            builder.Services.AddSingleton<ISerializer>(serializer);

            return builder;
        }

        /// <summary>
        /// Adds the assembly of the given type to the list of assemblies that are used for creating
        /// message instances when they are deserialized.
        /// </summary>
        public static IMeceqsBuilder AddDeserializationAssembly<TType>(this IMeceqsBuilder builder)
        {
            var assembly = typeof(TType).GetTypeInfo().Assembly;

            return builder.AddDeserializationAssembly(assembly);
        }

        /// <summary>
        /// Adds the assembly to the list of assemblies that are used for creating message instances
        /// when they are deserialized.
        /// </summary>
        public static IMeceqsBuilder AddDeserializationAssembly(this IMeceqsBuilder builder, params Assembly[] assemblies)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(assemblies, nameof(assemblies));

            builder.Services.Configure<EnvelopeTypeLoaderOptions>(options => options.TryAddContractAssembly(assemblies));

            return builder;
        }
    }
}