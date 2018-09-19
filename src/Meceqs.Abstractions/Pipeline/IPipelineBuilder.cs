using System;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Defines a class that provides the mechanisms to configure a pipeline.
    /// </summary>
    public interface IPipelineBuilder
    {
        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> that provides access to the application's service container.
        /// </summary>
        IServiceProvider ApplicationServices { get; }

        /// <summary>
        /// Adds the given middleware delegate to the pipeline.
        /// </summary>
        IPipelineBuilder Use(Func<MiddlewareDelegate, MiddlewareDelegate> middleware);

        /// <summary>
        /// Allows modifying the pipeline before it is built. This can be used to set the last middleware.
        /// </summary>
        IPipelineBuilder EndsWith(Action<IPipelineBuilder> onBuildPipeline);

        /// <summary>
        /// Creates an executable pipeline with all configured middleware components.
        /// </summary>
        MiddlewareDelegate Build();
    }
}
