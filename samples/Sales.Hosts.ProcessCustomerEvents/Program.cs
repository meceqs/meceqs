using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleConfig;

namespace Sales.Hosts.ProcessCustomerEvents
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
                    services.AddHostedService<EventProcessorService>();

                    services.AddOptions();
                    services.AddMeceqs(builder =>
                    {
                        builder
                            .AddEventHubReceiver(receiver =>
                            {
                                receiver
                                    .SkipUnknownMessages()
                                    .UseTypedHandling(options =>
                                    {
                                        options.Handlers.Add<CustomerEventsHandler>();
                                    })
                                    // RunTypedHandling will be added at the end automatically!
                                    .ConfigurePipeline(x =>
                                    {
                                        x.UseAuditing();
                                    });
                            })
                            // Fake for the EventHubReceiver which will read events from a local file.
                            .AddFileFakeEventHubProcessor(options =>
                            {
                                options.Directory = SampleConfiguration.FileFakeEventHubDirectory;
                                options.ClearEventHubOnStart = true;
                                options.EventHubName = "customers";
                            });
                    });
                });
        }
    }
}
