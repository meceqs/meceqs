using System;
using System.Threading.Tasks;
using Meceqs.AspNetCore.Configuration;
using Meceqs.AspNetCore.Receiving;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NSwag;

namespace Meceqs.AspNetCore.Swagger
{
    public class SwaggerMiddleware
    {
        private const string SwaggerPath = "/swagger/v1/swagger.json";

        private readonly RequestDelegate _next;
        private readonly AspNetCoreReceiverOptions _options;
        private readonly IMessagePathConvention _messagePathConvention;
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly object _lock = new object();

        private string _swaggerJson = null;

        public SwaggerMiddleware(
            RequestDelegate next,
            IOptions<AspNetCoreReceiverOptions> options,
            IMessagePathConvention messagePathConvention,
            IHostingEnvironment hostingEnvironment)
        {
            _next = next;
            _options = options.Value;
            _messagePathConvention = messagePathConvention;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task Invoke(HttpContext context)
        {
            if (string.Equals(context.Request.Path, SwaggerPath, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(GenerateSwagger(context));
            }
            else
            {
                await _next(context);
            }
        }

        /// <summary>Generates the Swagger specification.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The Swagger specification.</returns>
        protected virtual string GenerateSwagger(HttpContext context)
        {
            if (_swaggerJson == null)
            {
                lock (_lock)
                {
                    if (_swaggerJson == null)
                    {
                        var settings = new MeceqsToSwaggerGeneratorSettings();

                        var generator = new MeceqsToSwaggerGenerator(settings, _options, _messagePathConvention, _hostingEnvironment);
                        var document = generator.CreateSwaggerDocument();

                        document.Host = context.Request.Host.Value ?? "";
                        document.Schemes.Add(context.Request.Scheme == "http" ? SwaggerSchema.Http : SwaggerSchema.Https);
                        document.BasePath = "/" + context.Request.PathBase; //.Value?.Substring(0, context.Request.PathBase.Value.Length - _settings.MiddlewareBasePath?.Length ?? 0) ?? "";

                        _swaggerJson = document.ToJson();
                    }
                }
            }

            return _swaggerJson;
        }
    }
}