using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Meceqs.Pipeline
{
    public class DefaultPipelineBuilder : IPipelineBuilder
    {
        private readonly IList<Func<FilterDelegate, FilterDelegate>> _filters = new List<Func<FilterDelegate, FilterDelegate>>();
        private readonly string _pipelineName;
        private readonly ILoggerFactory _loggerFactory;

        public IServiceProvider ApplicationServices { get; }

        public DefaultPipelineBuilder(string pipelineName, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            ApplicationServices = serviceProvider;
            _pipelineName = pipelineName;
            _loggerFactory = loggerFactory;
        }

        public IPipelineBuilder Use(Func<FilterDelegate, FilterDelegate> filter)
        {
            _filters.Add(filter);
            return this;
        }

        public IPipeline Build()
        {
            FilterDelegate pipeline = context =>
            {
                // This filter will be executed last!
                throw new InvalidOperationException("The message has not been handled by a terminating filter");
            };

            foreach (var filter in _filters.Reverse())
            {
                pipeline = filter(pipeline);
            }

            return new DefaultPipeline(pipeline, _pipelineName, _loggerFactory);
        }
    }
}