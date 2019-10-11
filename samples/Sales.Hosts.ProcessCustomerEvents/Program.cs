using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<EventProcessorService>();

                    services.AddMeceqs(builder =>
                    {
                        builder
                            .AddEventHubReceiver(receiver =>
                            {
                                receiver.SkipUnknownMessages();

                                // This adds a custom middleware to the pipeline.
                                // They are executed in order of registration before the Typed Handling middleware is executed.
                                receiver.ConfigurePipeline(pipeline =>
                                {
                                    pipeline.UseAuditing();
                                });

                                // Process messages with `IHandles<...>`-implementations.
                                receiver.UseTypedHandling(options =>
                                {
                                    options.Handlers.Add<CustomerEventsHandler>();
                                });

                                if (hostContext.HostingEnvironment.IsDevelopment())
                                {
                                    // For this sample, we will read events from a local file instead of a real Event Hub.
                                    receiver.UseFileFake(options =>
                                    {
                                        options.Directory = SampleConfiguration.FileFakeEventHubDirectory;
                                        options.ClearEventHubOnStart = true;
                                        options.EventHubName = "customers";
                                    });
                                }
                            });
                    });
                });
        }
    }
}
