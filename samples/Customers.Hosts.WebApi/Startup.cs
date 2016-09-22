using Customers.Core.CommandHandlers;
using Customers.Core.Repositories;
using Customers.Hosts.WebApi.Infrastructure;
using Meceqs.AzureEventHubs.Sending;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleConfig;

namespace Customers.Hosts.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Customers.Core
            services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();

            ConfigureMeceqs(services);
        }

        private void ConfigureMeceqs(IServiceCollection services)
        {
            // Meceqs resolves interceptors transiently by default.
            // To change this, you can add the interceptor with your own lifecycle
            // to the DI framework and use "Interceptors.AddService()" to
            // tell Meceqs to resolve it from there.
            services.AddSingleton<SingletonHandleInterceptor>();

            services.Configure<EventHubSenderOptions>(x => x.EventHubConnectionString = "customers|dummy");

            services.AddMeceqs()

                // This allows all components to use JSON serialization.
                .AddJsonSerialization()

                // Pass HttpContext.RequestServices, .User, etc to every processed message.
                // (this will also be called by AddAspNetCoreConsumer so it wouldn't be necessary here,
                // it's just here for clarity).
                .AddAspNetCore()

                // The Web API will process incoming requests.
                .AddAspNetCoreConsumer(consumer =>
                {
                    // Throwing an exception is the default behavior.
                    // We could also skip unknown message types but this doesn't make much sense
                    // for a Web API.
                    consumer.ThrowOnUnknownMessage();

                    consumer.UseTypedHandling(options =>
                    {
                        // In this example, the context only handles messages from this Web API
                        // so we can just add every handler.
                        options.Handlers.AddFromAssembly<CustomerCommandHandler>();

                        // Interceptors know about the executing handler
                        // (e.g. to check for attributes on the handler)

                        // This interceptor is created for each message.
                        // (It doesn't need to be registered in the DI framework
                        // because Meceqs uses ActivatorUtilities to create the instance.)
                        options.Interceptors.Add<SampleHandleInterceptor>();

                        // this interceptor will use the lifecycle from the DI framework.
                        options.Interceptors.AddService<SingletonHandleInterceptor>();
                    });

                    // This adds some custom filters to the pipeline.
                    // They are executed in this order before the Typed Handling filter is executed.
                    consumer.ConfigurePipeline(pipeline =>
                    {
                        // add user id to message if not present
                        pipeline.UseAuditing();
                    });
                })

                // This Web API will also send messages to an Azure Event Hub.
                .AddEventHubSender(sender =>
                {
                    sender.ConfigurePipeline(pipeline => {
                        pipeline.UseAuditing(); // add user id to message if not present
                    });
                })

                // Fake for the EventHubSender which will send events to a local file.
                .AddFileFakeEventHubSender(SampleConfiguration.FileFakeEventHubDirectory);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseAspNetCoreConsumer();
        }
    }
}