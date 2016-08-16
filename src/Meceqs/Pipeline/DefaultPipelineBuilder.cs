using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    public class DefaultPipelineBuilder : IPipelineBuilder
    {
        private readonly IList<Func<FilterDelegate, FilterDelegate>> _filters = new List<Func<FilterDelegate, FilterDelegate>>();
        private readonly string _pipelineName;

        public IServiceProvider ApplicationServices { get; }

        public DefaultPipelineBuilder(IServiceProvider serviceProvider, string pipelineName)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            ApplicationServices = serviceProvider;
            _pipelineName = pipelineName;
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
                // TODO what should happen if there's no terminating filter?
                return Task.CompletedTask;
            };

            foreach (var filter in _filters.Reverse())
            {
                pipeline = filter(pipeline);
            }

            return new DefaultPipeline(pipeline, _pipelineName);
        }
    }
}