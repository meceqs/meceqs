using Customers.Contracts.Commands;
using Customers.Contracts.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sales.Contracts.Commands;
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
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<TrafficGeneratorService>();

                    services.AddMeceqs(hostContext.Configuration.GetSection("Meceqs"), builder =>
                    {
                        builder
                            .AddHttpSender("HttpSenderCustomers", sender =>
                            {
                                // Bind these messages to this pipeline
                                sender.AddMessageType<ChangeNameCommand>();
                                sender.AddMessageType<CreateCustomerCommand>();
                                sender.AddMessageType<FindCustomersQuery>();
                                sender.AddMessageType<GetCustomerQuery>();

                                // Adds an "Authorization" header for each request.
                                sender.HttpClient.AddHttpMessageHandler(() => new AuthorizationDelegatingHandler());
                            })
                            .AddServiceBusSender("ServiceBusSender", sender =>
                            {
                                // Bind these messages to this pipeline
                                sender.AddMessageType<PlaceOrderCommand>();

                                if (hostContext.HostingEnvironment.IsDevelopment())
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
