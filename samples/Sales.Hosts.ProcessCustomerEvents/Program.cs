using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sales.Hosts.ProcessCustomerEvents
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

            services.AddMeceqs()
                .AddJsonSerialization()

                .AddEventHubConsumer(options =>
                {
                    options.SkipUnknownMessages();
                    options.AddTypedHandler<CustomerEventsHandler>();
                });
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Sales.Hosts.ProcessCustomerEvents!");
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
