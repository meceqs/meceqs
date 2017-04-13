using System;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.AzureServiceBus.FileFake;
using Meceqs.AzureServiceBus.Internal;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FileFakeServiceBusMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddFileFakeServiceBusSender(this IMeceqsBuilder builder, string directory)
        {
            Guard.NotNull(builder, nameof(builder));

            builder.Services.AddSingleton<IServiceBusMessageSenderFactory>(serviceProvider =>
            {
                return new FileFakeServiceBusMessageSenderFactory(
                    directory,
                    serviceProvider.GetRequiredService<ILoggerFactory>());
            });

            return builder;
        }

        public static IMeceqsBuilder AddFileFakeServiceBusProcessor(this IMeceqsBuilder builder, Action<FileFakeServiceBusProcessorOptions> options)
        {
            Guard.NotNull(builder, nameof(builder));

            var processorOptions = new FileFakeServiceBusProcessorOptions();
            options?.Invoke(processorOptions);

            builder.Services.AddSingleton(processorOptions);
            builder.Services.AddSingleton<FileFakeServiceBusProcessor>();

            builder.Services.AddSingleton<IServiceBusMessageInvoker, FileFakeServiceBusMessageInvoker>();

            return builder;
        }
    }
}