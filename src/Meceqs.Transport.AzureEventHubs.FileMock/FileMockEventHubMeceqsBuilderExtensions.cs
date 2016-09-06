using Meceqs;
using Meceqs.Configuration;
using Meceqs.Transport.AzureEventHubs.FileMock;
using Meceqs.Transport.AzureEventHubs.Internal;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FileMockEventHubMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddFileMockEventHubClient(this IMeceqsBuilder builder, string directory)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.AddSingleton<IEventHubClientFactory>(serviceProvider =>
            {
                return new FileMockEventHubClientFactory(
                    directory,
                    serviceProvider.GetRequiredService<ILoggerFactory>());
            });

            return builder;
        }

        public static IMeceqsBuilder AddFileMockEventHubProcessor(this IMeceqsBuilder builder, string directory, string eventHubName)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new FileMockEventHubProcessorOptions
            {
                Directory = directory,
                EventHubName = eventHubName
            };

            builder.Services.AddSingleton(options);
            builder.Services.AddSingleton<FileMockEventHubProcessor>();

            return builder;
        }
    }
}