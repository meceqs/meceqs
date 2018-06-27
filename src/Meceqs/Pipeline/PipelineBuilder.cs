using System;
using System.Collections.Generic;
using System.Linq;

namespace Meceqs.Pipeline
{

    /// <summary>
    /// Used for configuring the middleware components of a pipeline.
    /// </summary>
    public class PipelineBuilder : IPipelineBuilder
    {
        private readonly IList<Func<MiddlewareDelegate, MiddlewareDelegate>> _components;

        private Action<IPipelineBuilder> _onBuildPipeline;

        public IServiceProvider ApplicationServices { get; }

        public PipelineBuilder(IServiceProvider serviceProvider)
        {
            Guard.NotNull(serviceProvider, nameof(serviceProvider));

            ApplicationServices = serviceProvider;

            _components = new List<Func<MiddlewareDelegate, MiddlewareDelegate>>();
        }

        /// <summary>
        /// Allows modifying the pipeline before it is built. This can be used to set the last middleware.
        /// </summary>
        public IPipelineBuilder EndsWith(Action<IPipelineBuilder> onBuildPipeline)
        {
            _onBuildPipeline = onBuildPipeline;
            return this;
        }

        /// <summary>
        /// Adds the given middleware delegate to the pipeline.
        /// </summary>
        public IPipelineBuilder Use(Func<MiddlewareDelegate, MiddlewareDelegate> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        /// <summary>
        /// Creates an executable pipeline with all configured middleware components.
        /// </summary>
        public MiddlewareDelegate Build()
        {
            _onBuildPipeline?.Invoke(this);

            // This middleware will always be the last one!
            MiddlewareDelegate pipeline = context => throw new InvalidOperationException("The message has not been handled by a terminating middleware");

            foreach (var middleware in _components.Reverse())
            {
                pipeline = middleware(pipeline);
            }

            return pipeline;
        }
    }
}