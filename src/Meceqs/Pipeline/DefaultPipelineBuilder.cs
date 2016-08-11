using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    public class DefaultPipelineBuilder : IPipelineBuilder
    {
        private readonly IList<Func<FilterDelegate, FilterDelegate>> _filters = new List<Func<FilterDelegate, FilterDelegate>>();

        public IServiceProvider ApplicationServices { get; set; }

        public DefaultPipelineBuilder(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            ApplicationServices = serviceProvider;
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
                // TODO what should happen if there's no terminating filter?
                return Task.CompletedTask;
            };

            foreach (var filter in _filters.Reverse())
            {
                pipeline = filter(pipeline);
            }

            return new DefaultPipeline(pipeline, pipelineName);
        }
    }
}