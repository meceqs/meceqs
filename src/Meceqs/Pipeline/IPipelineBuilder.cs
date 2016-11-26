using System;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Used for configuring the middleware components of a pipeline.
    /// </summary>
    public interface IPipelineBuilder
    {
        /// <summary>
        /// <para>The root service provider of your application.</para>
        /// <para>It will be used to do dependency injection on the constructors of your middleware components.</para>
        /// </summary>
        IServiceProvider ApplicationServices { get; }

        /// <summary>
        /// Adds the given middleware delegate to the pipeline.
        /// </summary>
        IPipelineBuilder Use(Func<MiddlewareDelegate, MiddlewareDelegate> middleware);

        /// <summary>
        /// Creates an executable pipeline with all configured middleware components.
        /// </summary>
        IPipeline Build(string pipelineName);
    }
}