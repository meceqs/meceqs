using Meceqs;
using Meceqs.Filters.DataAnnotationsValidator;
using Meceqs.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PipelineBuilderDataAnnotationsValidatorExtensions
    {
        public static IPipelineBuilder UseDataAnnotationsValidator(this IPipelineBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            return builder.UseFilter<DataAnnotationsValidatorFilter>();
        }
    }
}