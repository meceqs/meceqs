using System.Reflection;
using Customers.Core.CommandHandlers;
using Customers.Core.Repositories;
using Customers.Hosts.WebApi.Infrastructure;
using Meceqs.Transport.AzureEventHubs.Internal;
using Meceqs.Transport.AzureEventHubs.Sending;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Model;

namespace Customers.Hosts.WebApi
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
            // Meceqs resolves interceptors transiently by default.
            // To change this, you can add the interceptor with your own lifecycle
            // to the DI framework and use "Interceptors.AddService()" to
            // tell Meceqs to resolve it from there.
            services.AddSingleton<SingletonHandleInterceptor>();

            // Fake for the EventHubSender.
            // It will write the events to a local file (see "SampleConfig"-project for paths).
            services.Configure<EventHubSenderOptions>(x => x.EventHubConnectionString = "dummy|dummy");
            services.AddSingleton<IEventHubClientFactory, FakeEventHubClientFactory>();

            services.AddMeceqs()

                // Add services to Dependency Injection
                .AddAspNetCore()
                .AddTypedHandlersFromAssembly<CustomerCommandHandler>()
                .AddJsonSerialization()

                // The WebAPI receives messages through Controller actions
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

                            // This interceptor is created for each message.
                            // (It doesn't need to be registered in the DI framework
                            // because Meceqs uses ActivatorUtilities to create the instance.)
                            options.Interceptors.Add<SampleHandleInterceptor>();

                            // this interceptor will use the lifecycle from the DI framework.
                            options.Interceptors.AddService<SingletonHandleInterceptor>();
                        });
                })

                .AddEventHubSender(pipeline =>
                {
                    pipeline
                        .UseAspNetCoreRequest()
                        .UseAuditing()              // add user id to message if not present
                        .RunEventHubSender();
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