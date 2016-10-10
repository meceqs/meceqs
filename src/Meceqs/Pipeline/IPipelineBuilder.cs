using System;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Used for configuring the filters of a pipeline.
    /// </summary>
    public interface IPipelineBuilder
    {
        /// <summary>
        /// <para>The root service provider of your application.</para>
        /// <para>It will be used to do dependency injection on the constructors of your filters.</para>
        /// </summary>
        IServiceProvider ApplicationServices { get; }

        /// <summary>
        /// Adds the given filter delegate to the pipeline.
        /// </summary>
        IPipelineBuilder Use(Func<FilterDelegate, FilterDelegate> filter);

        /// <summary>
        /// Creates a pipeline with all configured filters.
        /// </summary>
        IPipeline Build(string pipelineName);
    }
}