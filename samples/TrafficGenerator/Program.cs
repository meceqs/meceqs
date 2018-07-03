using System;
using System.IO;
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
            // The generic HostBuilder does not have logic for automatically reading the environment.
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(environment))
            {
                environment = "Production";
            }

            // Based on https://github.com/aspnet/MetaPackages/blob/dev/src/Microsoft.AspNetCore/WebHost.cs
            return new HostBuilder()
                .UseEnvironment(environment)
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
                    services.AddHostedService<TrafficGeneratorService>();

                    services.AddMeceqs(hostingContext.Configuration.GetSection("Meceqs"), builder =>
                    {
                        builder
                            .AddHttpSender("HttpSenderCustomers", sender =>
                            {
                                // Adds an "Authorization" header for each request.
                                sender.HttpClient.AddHttpMessageHandler(() => new AuthorizationDelegatingHandler());
                            })
                            .AddServiceBusSender("ServiceBusSender", sender =>
                            {
                                if (hostingContext.HostingEnvironment.IsDevelopment())
                                {
                                    // For this sample, we will send messages to a local file instead of a real Service Hub.
                                    sender.UseFileFake(SampleConfiguration.FileFakeServiceBusDirectory, SampleConfiguration.PlaceOrderQueue);
                                }
                            });
                    });
                });
        }
    }
}
