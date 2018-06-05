using System;
using System.Collections.Generic;
using System.Linq;

namespace Meceqs.Pipeline
{
    /// <summary>
    /// Used for configuring the middleware components of a pipeline.
    /// </summary>
    public class PipelineOptions
    {
        private readonly IList<Func<MiddlewareDelegate, IServiceProvider, MiddlewareDelegate>> _middlewareEntries;

        private Action<PipelineOptions> _onBuildPipeline;

        public PipelineOptions()
        {
            _middlewareEntries = new List<Func<MiddlewareDelegate, IServiceProvider, MiddlewareDelegate>>();
        }

        /// <summary>
        /// Allows modifying the pipeline before it is built. This can be used to set the last middleware.
        /// </summary>
        public PipelineOptions EndsWith(Action<PipelineOptions> onBuildPipeline)
        {
            _onBuildPipeline = onBuildPipeline;
            return this;
        }

        /// <summary>
        /// Adds the given middleware delegate to the pipeline.
        /// </summary>
        public PipelineOptions Use(Func<MiddlewareDelegate, IServiceProvider, MiddlewareDelegate> middleware)
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

            if (_middlewareEntries.Count == 0)
            {
                return null;
            }

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