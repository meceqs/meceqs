using System;

namespace Meceqs.Pipeline
{
    public interface IPipelineBuilder
    {
        IServiceProvider ApplicationServices { get; }

        IPipelineBuilder Use(Func<FilterDelegate, FilterDelegate> filter);

        IPipeline Build(string pipelineName);
    }
}