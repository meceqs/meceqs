using System.IO;
using Customers.Core.Repositories;
using Customers.Core.CommandHandlers;
using Customers.Hosts.WebApi.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SampleConfig;
using Meceqs.AzureEventHubs.Sending;

namespace Customers.Hosts.WebApi
{
    public class Program
    {
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
                })
                .ConfigureMeceqs(meceqs =>
                {
                    // Meceqs resolves interceptors transiently by default.
                    // To change this, you can add the interceptor with your own lifecycle
                    // to the DI framework and use "Interceptors.AddService()" to
                    // tell Meceqs to resolve it from there.
                    meceqs.Services.AddSingleton<SingletonHandleInterceptor>();

                    meceqs.Services.Configure<EventHubSenderOptions>(x => x.EventHubConnectionString = "Endpoint=sb://dummy;EntityPath=customers");

                    meceqs

                        // The Web API will process incoming requests.
                        .AddAspNetCoreReceiver(receiver =>
                        {
                            // Throwing an exception is the default behavior.
                            // We could also skip unknown message types but this doesn't make much sense
                            // for a Web API.
                            receiver.ThrowOnUnknownMessage();

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

                            // This adds a custom middleware to the pipeline.
                            // They are executed in this order before the Typed Handling middleware is executed.
                            receiver.ConfigurePipeline(pipeline =>
                            {
                                // add user id to message if not present
                                pipeline.UseAuditing();
                            });
                        })

                        // This Web API will also send messages to an Azure Event Hub.
                        .AddEventHubSender(sender =>
                        {
                            sender.ConfigurePipeline(pipeline =>
                            {
                                pipeline.UseAuditing(); // add user id to message if not present
                            });
                        })

                        // Fake for the EventHubSender which will send events to a local file.
                        .AddFileFakeEventHubSender(SampleConfiguration.FileFakeEventHubDirectory);
                })
                .Configure(app =>
                {
                    app.UseDeveloperExceptionPage();
                    app.UseAspNetCoreReceiverWithSwagger();
                });
        }
    }
}