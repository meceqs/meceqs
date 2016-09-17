using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Meceqs.Pipeline
{
    public class DefaultPipelineBuilder : IPipelineBuilder
    {
        private readonly IList<Func<FilterDelegate, FilterDelegate>> _filters = new List<Func<FilterDelegate, FilterDelegate>>();
        private readonly ILoggerFactory _loggerFactory;
        private readonly IFilterContextEnricher _filterContextEnricher;

        public IServiceProvider ApplicationServices { get; }

        public DefaultPipelineBuilder(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            IFilterContextEnricher filterContextEnricher = null)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            ApplicationServices = serviceProvider;
            _loggerFactory = loggerFactory;
            _filterContextEnricher = filterContextEnricher;
        }

        public IPipelineBuilder Use(Func<FilterDelegate, FilterDelegate> filter)
        {
            _filters.Add(filter);
            return this;
        }

        public IPipeline Build(string pipelineName)
        {
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            FilterDelegate pipeline = context =>
            {
                // This filter will be executed last!
                throw new InvalidOperationException("The message has not been handled by a terminating filter");
            };

            foreach (var filter in _filters.Reverse())
            {
                pipeline = filter(pipeline);
            }

            return new DefaultPipeline(pipeline, pipelineName, _loggerFactory, _filterContextEnricher);
        }
    }
}