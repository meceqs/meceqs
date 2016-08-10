using System;

namespace Meceqs.Pipeline
{
    public interface IPipelineBuilder
    {
        IServiceProvider ApplicationServices { get; set; }

        IPipelineBuilder Use(Func<FilterDelegate, FilterDelegate> filter);

        FilterDelegate Build();
    }
}