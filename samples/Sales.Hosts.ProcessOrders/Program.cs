using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleConfig;

namespace Sales.Hosts.ProcessOrders
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
                    services.AddHostedService<ServiceBusProcessorService>();

                    services.AddMeceqs(builder =>
                    {
                        builder
                            .AddServiceBusReceiver(receiver =>
                            {
                                receiver.UseTypedHandling(options =>
                                {
                                    options.Handlers.AddFromAssembly<Program>();
                                });

                                if (hostingContext.HostingEnvironment.IsDevelopment())
                                {
                                    // Will read messages from local file system.
                                    receiver.UseFileFake(options =>
                                    {
                                        options.Directory = SampleConfiguration.FileFakeServiceBusDirectory;
                                        options.EntityPath = SampleConfiguration.PlaceOrderQueue;
                                    });
                                }
                            });
                    });
                });
        }
    }
}
