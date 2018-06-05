using System;
using Meceqs;
using Meceqs.AzureEventHubs.FileFake;
using Meceqs.AzureEventHubs.Internal;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FileFakeEventHubBuilderExtensions
    {
        public static IEventHubSenderBuilder UseFileFake(this IEventHubSenderBuilder builder, string directory, string entityPath)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNullOrWhiteSpace(directory, nameof(directory));
            Guard.NotNullOrWhiteSpace(entityPath, nameof(entityPath));

            builder.SetConnectionString("Endpoint=sb://dummy;EntityPath=" + entityPath);

            builder.Services.AddSingleton<IEventHubClientFactory>(serviceProvider =>
            {
                return new FileFakeEventHubClientFactory(
                    directory,
                    serviceProvider.GetRequiredService<ILoggerFactory>());
            });

            return builder;
        }

        public static IEventHubReceiverBuilder UseFileFake(this IEventHubReceiverBuilder builder, Action<FileFakeEventHubProcessorOptions> options)
        {
            Guard.NotNull(builder, nameof(builder));

            var processorOptions = new FileFakeEventHubProcessorOptions();
            options?.Invoke(processorOptions);

            builder.Services.AddSingleton(processorOptions);
            builder.Services.AddSingleton<FileFakeEventHubProcessor>();

            return builder;
        }
    }
}