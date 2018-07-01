using System.IO;
using Customers.Contracts.Commands;
using Meceqs.AzureServiceBus.Sending;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleConfig;

namespace TrafficGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            // Based on https://github.com/aspnet/MetaPackages/blob/dev/src/Microsoft.AspNetCore/WebHost.cs
            return new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddOptions();
                    services.AddHostedService<TrafficGeneratorService>();

                    services.Configure<ServiceBusSenderOptions>(x => {
                        x.ConnectionString = $"Endpoint=sb://dummy.example.com;EntityPath={SampleConfiguration.PlaceOrderQueue}";
                    });

                    services.AddMeceqs(builder =>
                    {
                        builder
                            .AddHttpSender(sender =>
                            {
                                sender.SetBaseAddress(SampleConfiguration.CustomersWebApiUrl + "v1/");

                                // Adds an "Authorization" header for each request.
                                sender.HttpClient.AddHttpMessageHandler<AuthorizationDelegatingHandler>();

                                // Write your own extension method if you have a base class for alle messages
                                sender.AddMessagesFromAssembly<CreateCustomerCommand>(t => t.Name.EndsWith("Command") || t.Name.EndsWith("Query"));
                            })
                            .AddServiceBusSender("ServiceBus", sender =>
                            {
                                // For this sample, we will send messages to a local file instead of a real Service Hub.
                                sender.UseFileFake(SampleConfiguration.FileFakeServiceBusDirectory, SampleConfiguration.PlaceOrderQueue);
                            });
                    });
                });
        }
    }
}
