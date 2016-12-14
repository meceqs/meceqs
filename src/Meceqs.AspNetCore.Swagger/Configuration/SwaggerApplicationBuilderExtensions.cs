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
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, Action<SwaggerUiOptions> options)
        {
            Check.NotNull(app, nameof(app));
            Check.NotNull(options, nameof(options));

            var swaggerUiOptions = new SwaggerUiOptions();
            options?.Invoke(swaggerUiOptions);

            return UseSwagger(app, swaggerUiOptions);
        }

        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, SwaggerUiOptions options = null)
        {
            Check.NotNull(app, nameof(app));

            options = options ?? new SwaggerUiOptions();

            if (options.IndexConfig.JSConfig.SwaggerEndpoints.Count == 0)
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Definition");
            }

            // Swagger Definition
            app.UseMiddleware<SwaggerMiddleware>();

            // Swagger UI redirect from BasePath
            app.UseMiddleware<RedirectMiddleware>(options.BaseRoute, options.IndexPath);

            // Custom Swagger UI index file
            app.UseMiddleware<SwaggerUiMiddleware>(options);

            // Swagger UI static files
            var fileServerOptions = new FileServerOptions
            {
                RequestPath = $"/{options.BaseRoute}",
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