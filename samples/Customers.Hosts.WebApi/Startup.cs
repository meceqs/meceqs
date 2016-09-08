using System.Reflection;
using Customers.Core.CommandHandlers;
using Customers.Core.Repositories;
using Customers.Hosts.WebApi.Infrastructure;
using Meceqs.Configuration;
using Meceqs.AzureEventHubs.Sending;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleConfig;
using Swashbuckle.Swagger.Model;
using Customers.Contracts.Queries;

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
            // Customers.Core
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

            services.Configure<EventHubSenderOptions>(x => x.EventHubConnectionString = "customers|dummy");

            services.AddMeceqs()

                // Add services to Dependency Injection
                .AddAspNetCore()
                .AddJsonSerialization()
                .AddDeserializationAssembly<FindCustomersQuery>()

                // The WebApi is a consumer of remote messages.
                .AddAspNetCoreConsumer()
                .AddConsumer(pipeline =>
                {
                    pipeline
                        .UseEnvelopeSanitizer()     // make sure, MessageType, MessageName, etc are set correctly
                        .UseAspNetCoreRequest()     // attach User, RequestServices, MessageHistory, ...
                        .UseAuditing()              // add user id to message if not present

                        // forward to IHandles<TMessage, TResult>
                        .RunTypedHandling(options =>
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

                            // Throwing an exception is the default behavior.
                            // We could also skip unknown message types but this doesn't make much sense
                            // for a Web API.
                            options.UnknownMessageBehavior = UnknownMessageBehavior.ThrowException;
                        });
                })

                .AddEventHubSender(pipeline =>
                {
                    pipeline
                        .UseAspNetCoreRequest()
                        .UseAuditing()              // add user id to message if not present
                        .RunEventHubSender();
                })
                
                // Fake for the EventHubSender which will send events to a local file.
                .AddFileFakeEventHubSender(SampleConfiguration.FileFakeEventHubDirectory);
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

            app.UseAspNetCoreConsumer(options =>
            {
                options.AddMessageType<FindCustomersQuery, FindCustomersResult>();
            });

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUi();
        }
    }
}