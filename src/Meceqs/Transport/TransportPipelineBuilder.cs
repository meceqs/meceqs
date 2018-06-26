using Meceqs.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Transport
{
    public abstract class TransportPipelineBuilder
    {
        /// <summary>
        /// Gets the Meceqs builder.
        /// </summary>
        public IMeceqsBuilder MeceqsBuilder { get; }

        /// <summary>
        /// Gets the application service collection.
        /// </summary>
        public IServiceCollection Services => MeceqsBuilder.Services;

        /// <summary>
        /// Gets the configuration section for the pipeline being built.
        /// </summary>
        public IConfigurationSection PipelineConfiguration { get; }

        /// <summary>
        /// Allows adding custom middelware components to the pipeline.
        /// </summary>
        public PipelineBuilder Pipeline { get; }

        /// <summary>
        /// Gets the name of the pipeline configured by this builder.
        /// </summary>
        public string PipelineName => Pipeline.Name;

        protected TransportPipelineBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
        {
            Guard.NotNull(meceqsBuilder, nameof(meceqsBuilder));
            Guard.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            MeceqsBuilder = meceqsBuilder;
            Pipeline = new PipelineBuilder(pipelineName);
            PipelineConfiguration = meceqsBuilder.Configuration.GetSection(PipelineName);

            MeceqsBuilder.AddPipeline(Pipeline);
        }
    }
}