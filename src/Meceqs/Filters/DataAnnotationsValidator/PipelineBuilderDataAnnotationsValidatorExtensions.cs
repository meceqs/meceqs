using Meceqs.Filters.DataAnnotationsValidator;

namespace Meceqs.Pipeline
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