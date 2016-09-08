using System;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.AzureEventHubs.FileFake;
using Meceqs.AzureEventHubs.Internal;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FileFakeEventHubMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddFileFakeEventHubSender(this IMeceqsBuilder builder, string directory)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.AddSingleton<IEventHubClientFactory>(serviceProvider =>
            {
                return new FileFakeEventHubClientFactory(
                    directory,
                    serviceProvider.GetRequiredService<ILoggerFactory>());
            });

            return builder;
        }

        public static IMeceqsBuilder AddFileFakeEventHubProcessor(this IMeceqsBuilder builder, Action<FileFakeEventHubProcessorOptions> options)
        {
            Check.NotNull(builder, nameof(builder));

            var processorOptions = new FileFakeEventHubProcessorOptions();
            options?.Invoke(processorOptions);

            builder.Services.AddSingleton(processorOptions);
            builder.Services.AddSingleton<FileFakeEventHubProcessor>();

            return builder;
        }
    }
}