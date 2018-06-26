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
        // TODO @cweiss name it AddPipeline again and throw if it already exists?!

        /// <summary>
        /// Configures the default "Receive" pipeline. This pipeline will be used when <see cref="Meceqs.Receiving.IMessageReceiver"/>
        /// is used without specifying a named pipeline.
        /// </summary>
        public static IMeceqsBuilder ConfigureReceivePipeline(this IMeceqsBuilder builder, Action<PipelineOptions> pipeline)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(pipeline, nameof(pipeline));

            builder.ConfigurePipeline(MeceqsDefaults.ReceivePipelineName, pipeline);

            return builder;
        }

        /// <summary>
        /// Configures the default "Send" pipeline. This pipeline will be used when <see cref="Meceqs.Sending.IMessageSender"/>
        /// is used without specifying a named pipeline.
        /// </summary>
        public static IMeceqsBuilder ConfigureSendPipeline(this IMeceqsBuilder builder, Action<PipelineOptions> pipeline)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(pipeline, nameof(pipeline));

            builder.ConfigurePipeline(MeceqsDefaults.SendPipelineName, pipeline);

            return builder;
        }

        /// <summary>
        /// Configures a named pipeline. This pipeline can be used by <see cref="Meceqs.Receiving.IMessageReceiver"/>
        /// and <see cref="Meceqs.Sending.IMessageSender"/>.
        /// </summary>
        public static IMeceqsBuilder ConfigurePipeline(this IMeceqsBuilder builder, string pipelineName, Action<PipelineOptions> pipeline)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));
            Guard.NotNull(pipeline, nameof(pipeline));

            builder.Services.Configure(pipelineName, pipeline);

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