using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PipelineBuilderUseExtensions
    {
        /// <summary>
        /// Adds an in-line middleware to the pipeline.
        /// </summary>
        public static IPipelineBuilder Use(this IPipelineBuilder builder, Func<MessageContext, Func<Task>, Task> middleware)
        {
            return builder.Use(next =>
            {
                return context =>
                {
                    Func<Task> simpleNext = () => next(context);
                    return middleware(context, simpleNext);
                };
            });
        }
    }
}
