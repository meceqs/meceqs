using System;
using System.Collections.Generic;
using System.Linq;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Used for configuring the middleware components of a pipeline.
    /// </summary>
    public class PipelineBuilder
    {
        private readonly IList<Func<MiddlewareDelegate, IServiceProvider, MiddlewareDelegate>> _middlewareEntries;

        private Action<PipelineBuilder> _onBuildPipeline;

        /// <summary>
        /// The name of the pipeline being built.
        /// </summary>
        public string Name { get; }

        public PipelineBuilder(string pipelineName)
        {
            Guard.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            Name = pipelineName;
            _middlewareEntries = new List<Func<MiddlewareDelegate, IServiceProvider, MiddlewareDelegate>>();
        }

        /// <summary>
        /// Allows modifying the pipeline before it is built. This can be used to set the last middleware.
        /// </summary>
        public PipelineBuilder EndsWith(Action<PipelineBuilder> onBuildPipeline)
        {
            _onBuildPipeline = onBuildPipeline;
            return this;
        }

        /// <summary>
        /// Adds the given middleware delegate to the pipeline.
        /// </summary>
        public PipelineBuilder Use(Func<MiddlewareDelegate, IServiceProvider, MiddlewareDelegate> middleware)
        {
            _middlewareEntries.Add(middleware);
            return this;
        }

        /// <summary>
        /// Creates an executable pipeline with all configured middleware components.
        /// </summary>
        public MiddlewareDelegate BuildPipeline(IServiceProvider applicationServices)
        {
            _onBuildPipeline?.Invoke(this);

            // This middleware will always be the last one!
            MiddlewareDelegate pipeline = context => throw new InvalidOperationException("The message has not been handled by a terminating middleware");

            foreach (var middleware in _middlewareEntries.Reverse())
            {
                pipeline = middleware(pipeline, applicationServices);
            }

            return pipeline;
        }
    }
}