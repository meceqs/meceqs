using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PipelineBuilderUseExtensions
    {
        /// <summary>
        /// In-line filter.
        /// </summary>
        public static IPipelineBuilder Use(this IPipelineBuilder builder, Func<FilterContext, Func<Task>, Task> filter)
        {
            return builder.Use(next => 
            {
                return context =>
                {
                    Func<Task> simpleNext = () => next(context);
                    return filter(context, simpleNext);
                };
            });
        }
    }
}