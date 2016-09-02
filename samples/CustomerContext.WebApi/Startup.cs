using System.Reflection;
using CustomerContext.Core.CommandHandlers;
using CustomerContext.Core.Repositories;
using CustomerContext.WebApi.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Model;

namespace CustomerContext.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // CustomerContext.Core
            services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();

            ConfigureMeceqs(services);

            ConfigureMvc(services);

            ConfigureSwagger(services);
        }

        private void ConfigureMeceqs(IServiceCollection services)
        {
            services.AddMeceqs()

                // Add services to Dependency Injection
                .AddAspNetCore()
                .AddTypedHandlersFromAssembly<CustomerCommandHandler>()

                // The WebAPI receives messages through Controllers
                .AddConsumer(pipeline =>
                {
                    pipeline
                        .UseEnvelopeSanitizer()     // make sure, MessageType, MessageName, etc are set correctly
                        .UseAspNetCoreRequest()     // attach User, RequestServices, MessageHistory, ...
                        .UseAuditing()              // add user id to message if not present

                        // forward to IHandles<TMessage, TResult>
                        .UseTypedHandling(options =>
                        {
                            // Interceptors know about the executing handler
                            // (e.g. to check for attributes on the handler)
                            options.Interceptors.Add<SampleHandleInterceptor>();
                        });
                })
                .AddSender(pipeline =>
                {
                    pipeline
                        .UseAspNetCoreRequest()
                        .UseAuditing()              // add user id to message if not present
                        .UseTypedHandling();
                });
        }

        private void ConfigureMvc(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(new RejectInvalidModelStateActionFilter());
                options.Conventions.Add(new BindComplexTypeFromBodyConvention());
            });
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = Assembly.GetEntryAssembly().GetName().Name
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            app.UseDeveloperExceptionPage();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUi();
        }
    }
}