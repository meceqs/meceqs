using System;
using System.Threading.Tasks;
using Customers.Contracts.Commands;
using Meceqs.AzureServiceBus.Sending;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleConfig;

namespace TrafficGenerator
{
    /// <summary>
    /// This class configures the DI framework and launches the <see cref="Worker"/>.
    /// </summary>
    public class Program
    {
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddOptions();

            services.Configure<ServiceBusSenderOptions>(x => {
                x.ConnectionString = "dummy";
                x.EntityPath = SampleConfiguration.PlaceOrderQueue;
            });

            services.AddMeceqs()
                .AddJsonSerialization()

                .AddHttpSender(sender =>
                {
                    sender.AddEndpoint("Customers", options =>
                    {
                        options.BaseAddress = SampleConfiguration.CustomersWebApiUrl;

                        // Write your own extension method if you have a base class for alle messages
                        options.AddMessagesFromAssembly<CreateCustomerCommand>(t => t.Name.EndsWith("Command") || t.Name.EndsWith("Query"));
                    });
                })

                .AddServiceBusSender("ServiceBus", pipeline =>
                {
                    pipeline.RunServiceBusSender();
                })

                // send messages to a local file instead of the actual Service Bus.
                .AddFileFakeServiceBusSender(SampleConfiguration.FileFakeServiceBusDirectory);
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("TrafficGenerator!");
            Console.WriteLine("==================================");
            Console.WriteLine();
            Console.WriteLine("Press [ENTER] to close application");
            Console.WriteLine();

            // Logging

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.Debug);

            var logger = loggerFactory.CreateLogger<Program>();

            // From now on, we can actually do something with startup exceptions

            IServiceProvider applicationServices = null;
            try
            {
                logger.LogInformation("Logging initialized");

                // Dependency Injection

                var services = new ServiceCollection();
                services.AddSingleton<ILoggerFactory>(loggerFactory);

                ConfigureServices(services);

                applicationServices = services.BuildServiceProvider();

                // Start Worker on new Thread

                var worker = (Worker)ActivatorUtilities.CreateInstance(applicationServices, typeof(Worker));
                Task.Factory.StartNew(worker.Run);

                // Wait for cancellation

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                logger?.LogError(0, ex, "Unhandled exception");
            }
            finally
            {
                // Close application

                logger.LogInformation("Closing application");
                (applicationServices as IDisposable)?.Dispose();
                loggerFactory?.Dispose();

                // Empty line just for nicer output in console.
                Console.WriteLine();
            }
        }
    }
}
