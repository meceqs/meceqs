using System;
using Meceqs;
using Meceqs.DependencyInjection;
using Meceqs.Pipeline;
using Meceqs.Receiving;
using Meceqs.Sending;
using Meceqs.Serialization;
using Meceqs.TypedHandling.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeceqsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Meceqs core services and returns a builder that allows to add or configure Meceqs features.
        /// </summary>
        /// <param name="services">The application services collection.</param>
        /// <param name="configuration">The configuration (section) which contains the configuration values for Meceqs.</param>
        public static IMeceqsBuilder AddMeceqs(this IServiceCollection services, IConfiguration configuration = null)
        {
            Guard.NotNull(services, nameof(services));

            // Core Services
            AddPipeline(services);
            AddReceiving(services);
            AddSending(services);
            AddSerialization(services);
            AddTypedHandling(services);

            var meceqsBuilder = new MeceqsBuilder(services, configuration);

            // Default Configuration
            meceqsBuilder.AddJsonSerialization();

            return meceqsBuilder;
        }

        /// <summary>
        /// Adds Meceqs core services and allows to add or configure Meceqs features.
        /// </summary>
        public static IServiceCollection AddMeceqs(this IServiceCollection services, Action<IMeceqsBuilder> builder)
        {
            return AddMeceqs(services, null, builder);
        }

        /// <summary>
        /// Adds Meceqs core services and allows to add or configure Meceqs features.
        /// </summary>
        /// /// <param name="services">The application services collection.</param>
        /// <param name="configuration">The configuration (section) which contains the configuration values for Meceqs.</param>
        /// <param name="builder"></param>
        public static IServiceCollection AddMeceqs(this IServiceCollection services, IConfiguration configuration, Action<IMeceqsBuilder> builder)
        {
            IMeceqsBuilder builderInstance = AddMeceqs(services, configuration);
            builder?.Invoke(builderInstance);
            return services;
        }

        private static void AddPipeline(IServiceCollection services)
        {
            services.TryAddSingleton<IPipelineProvider, DefaultPipelineProvider>();
        }

        private static void AddReceiving(IServiceCollection services)
        {
            // Resolving the receiver transiently makes sure it gets
            // resolved from a scoped service provider in case there is one.
            services.TryAddTransient<IMessageReceiver, MessageReceiver>();
        }

        private static void AddSending(IServiceCollection services)
        {
            services.TryAddSingleton<IEnvelopeFactory, DefaultEnvelopeFactory>();
            services.TryAddSingleton<IEnvelopeCorrelator, DefaultEnvelopeCorrelator>();

            // Resolving the sender transiently makes sure it gets
            // resolved from a scoped service provider in case there is one.
            services.TryAddTransient<IMessageSender, MessageSender>();
        }

        private static void AddSerialization(IServiceCollection services)
        {
            services.TryAddSingleton<ISerializationProvider, DefaultSerializationProvider>();
            services.TryAddSingleton<IEnvelopeTypeLoader, DefaultEnvelopeTypeLoader>();
        }

        public static void AddTypedHandling(IServiceCollection services)
        {
            services.TryAddSingleton<IHandleMethodResolver, DefaultHandleMethodResolver>();
            services.TryAddSingleton<IHandlerInvoker, DefaultHandlerInvoker>();
        }
    }
}