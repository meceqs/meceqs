using System;
using System.Reflection;
using Meceqs;
using Meceqs.AspNetCore.Swagger;
using Meceqs.AspNetCore.Swagger.SwaggerUi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SwaggerApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, Action<SwaggerUiOptions> options = null)
        {
            Check.NotNull(app, nameof(app));

            // Swagger Definition
            app.UseMiddleware<SwaggerMiddleware>();

            // Swagger UI Options

            var swaggerUiOptions = new SwaggerUiOptions();
            options?.Invoke(swaggerUiOptions);

            if (swaggerUiOptions.IndexConfig.JSConfig.SwaggerEndpoints.Count == 0)
            {
                swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "API Definition");
            }

            // Custom Swagger UI index file
            app.UseMiddleware<SwaggerUiMiddleware>(swaggerUiOptions);

            // Swagger UI static files
            var fileServerOptions = new FileServerOptions
            {
                RequestPath = $"/{swaggerUiOptions.BaseRoute}",
                EnableDefaultFiles = false,
                FileProvider = new EmbeddedFileProvider(
                    typeof(SwaggerApplicationBuilderExtensions).GetTypeInfo().Assembly,
                    "Meceqs.AspNetCore.Swagger.SwaggerUi.dist")
            };
            fileServerOptions.StaticFileOptions.ContentTypeProvider = new FileExtensionContentTypeProvider();
            app.UseFileServer(fileServerOptions);

            return app;
        }
    }
}