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

        public PipelineOptions()
        {
            _middlewareEntries = new List<Func<MiddlewareDelegate, IServiceProvider, MiddlewareDelegate>>();
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
        public MiddlewareDelegate BuildPipelineDelegate(IServiceProvider applicationServices)
        {
            if (_middlewareEntries.Count == 0)
            {
                return null;
            }

            MiddlewareDelegate pipeline = context =>
            {
                // This middleware will be executed last!
                throw new InvalidOperationException("The message has not been handled by a terminating middleware");
            };

            foreach (var middleware in _middlewareEntries.Reverse())
            {
                pipeline = middleware(pipeline, applicationServices);
            }

            return pipeline;
        }
    }
}