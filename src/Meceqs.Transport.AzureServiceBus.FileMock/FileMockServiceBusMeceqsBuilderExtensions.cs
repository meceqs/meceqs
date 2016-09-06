using System;
using Meceqs;
using Meceqs.Configuration;
using Meceqs.Transport.AzureServiceBus.FileMock;
using Meceqs.Transport.AzureServiceBus.Internal;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FileMockServiceBusMeceqsBuilderExtensions
    {
        public static IMeceqsBuilder AddFileMockServiceBusSender(this IMeceqsBuilder builder, string directory)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.AddSingleton<IServiceBusMessageSenderFactory>(serviceProvider =>
            {
                return new FileMockServiceBusMessageSenderFactory(
                    directory,
                    serviceProvider.GetRequiredService<ILoggerFactory>());
            });

            return builder;
        }

        public static IMeceqsBuilder AddFileMockServiceBusProcessor(this IMeceqsBuilder builder, Action<FileMockServiceBusProcessorOptions> options)
        {
            Check.NotNull(builder, nameof(builder));

            var processorOptions = new FileMockServiceBusProcessorOptions();
            options?.Invoke(processorOptions);

            builder.Services.AddSingleton(processorOptions);
            builder.Services.AddSingleton<FileMockServiceBusProcessor>();

            builder.Services.AddSingleton<IBrokeredMessageInvoker, FileMockBrokeredMessageInvoker>();

            return builder;
        }
    }
}