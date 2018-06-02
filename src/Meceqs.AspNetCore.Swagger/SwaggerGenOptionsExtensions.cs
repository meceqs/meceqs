using Meceqs.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SwaggerGenOptionsExtensions
    {
        public static SwaggerGenOptions AddMeceqs(this SwaggerGenOptions options, Action<MeceqsSwaggerOptions> meceqsOptions = null)
        {
            var meceqsOptionsInstance = new MeceqsSwaggerOptions();
            meceqsOptions?.Invoke(meceqsOptionsInstance);

            options.DocumentFilter<MeceqsDocumentFilter>(meceqsOptionsInstance);
            return options;
        }
    }

    public class MeceqsSwaggerOptions
    {
        /// <summary>
        /// Adds the given security definition and 401 &amp; 403 responses to each operation.
        /// </summary>
        public string SecurityDefinition { get; set; }
    }
}
