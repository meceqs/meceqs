using System;
using System.Collections.Generic;

namespace Meceqs.Pipeline
{
    public class PipelineOptions
    {
        public Dictionary<string, Action<IPipelineBuilder>> Pipelines { get; set; } = new Dictionary<string, Action<IPipelineBuilder>>();
    }
}