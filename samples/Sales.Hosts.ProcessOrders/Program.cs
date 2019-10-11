using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMeceqs(builder =>
                    {
                        builder
                            .AddServiceBusReceiver(receiver =>
                            {
                                receiver.UseTypedHandling(options =>
                                {
                                    options.Handlers.AddFromAssembly<Program>();
                                });

                                if (hostContext.HostingEnvironment.IsDevelopment())
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

                    services.AddHostedService<ServiceBusProcessorService>();
                });
        }
    }
}
