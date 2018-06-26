using Customers.Core.Repositories;
using Customers.Core.CommandHandlers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SampleConfig;
using Swashbuckle.AspNetCore.Swagger;

namespace Customers.Hosts.WebApi
{
    public class Program
    {
        private const string ApiVersion = "v1";
        private const string ApiName = "Customers API";

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder()
                .UseUrls(SampleConfiguration.CustomersWebApiUrl)
                .ConfigureServices(services =>
                {
                    // Customers.Core
                    services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();

                    // MVC (Only required if you need your own controllers/pages)
                    services.AddMvc();

                    services.AddSwaggerGen(options =>
                    {
                        // This registers all handlers that have been added to the AspNetCoreReceiver
                        options.AddMeceqs();

                        options.SwaggerDoc(ApiVersion, new Info
                        {
                            Title = ApiName,
                            Version = ApiVersion
                        });
                    });
                })
                .ConfigureMeceqs(meceqs =>
                {
                    // Meceqs resolves interceptors transiently by default.
                    // To change this, you can add the interceptor with your own lifecycle
                    // to the DI framework and use "Interceptors.AddService()" to
                    // tell Meceqs to resolve it from there.
                    meceqs.Services.AddSingleton<SingletonHandleInterceptor>();

                    meceqs
                        // The Web API will process incoming requests
                        .AddAspNetCoreReceiver(receiver =>
                        {
                            // Add a "/v1/" prefix to all message routes
                            receiver.SetRoutePrefix("/" + ApiVersion);

                            // This adds a custom middleware to the pipeline.
                            // They are executed in this order before the TypedHandling middleware is executed.
                            receiver.Pipeline.UseAuditing();// add user id to message if not present

                            // Process messages with `IHandles<...>`-implementations.
                            receiver.UseTypedHandling(options =>
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
                        })

                        // This Web API will also send messages to an Azure Event Hub.
                        .AddEventHubSender(sender =>
                        {
                            sender.Pipeline.UseAuditing(); // add user id to message if not present

                            // For this sample, we will send messages to a local file instead of a real Event Hub.
                            sender.UseFileFake(SampleConfiguration.FileFakeEventHubDirectory, "customers");
                        });
                })
                .Configure(app =>
                {
                    app.UseDeveloperExceptionPage();

                    app.UseMeceqs();

                    // MVC (Only required if you need your own controllers/pages)
                    app.UseMvc();

                    // Swagger
                    app.UseSwagger(options =>
                    {
                        options.RouteTemplate = "/swagger/{documentName}/swagger.json";
                    });
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint($"{ApiVersion}/swagger.json", $"{ApiName} - {ApiVersion}");
                    });
                });
        }
    }
}