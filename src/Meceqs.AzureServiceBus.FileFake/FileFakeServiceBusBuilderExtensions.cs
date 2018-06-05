using System;
using Meceqs;
using Meceqs.AzureServiceBus.FileFake;
using Meceqs.AzureServiceBus.Internal;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FileFakeServiceBusBuilderExtensions
    {
        public static IServiceBusSenderBuilder UseFileFake(this IServiceBusSenderBuilder builder, string directory, string entityPath)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNullOrWhiteSpace(directory, nameof(directory));
            Guard.NotNullOrWhiteSpace(entityPath, nameof(entityPath));

            builder.SetConnectionString($"Endpoint=sb://dummy.example.com;EntityPath={entityPath}");

            builder.Services.AddSingleton<IServiceBusMessageSenderFactory>(serviceProvider =>
            {
                return new FileFakeServiceBusMessageSenderFactory(
                    directory,
                    serviceProvider.GetRequiredService<ILoggerFactory>());
            });

            return builder;
        }

        public static IServiceBusReceiverBuilder UseFileFake(this IServiceBusReceiverBuilder builder, Action<FileFakeServiceBusProcessorOptions> options)
        {
            Guard.NotNull(builder, nameof(builder));

            var processorOptions = new FileFakeServiceBusProcessorOptions();
            options?.Invoke(processorOptions);

            builder.Services.AddSingleton(processorOptions);
            builder.Services.AddSingleton<FileFakeServiceBusProcessor>();

            return builder;
        }
    }
}