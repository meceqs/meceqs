using Meceqs;
using Meceqs.Configuration;
using Meceqs.Pipeline;
using Meceqs.Receiving;
using Meceqs.Sending;
using Meceqs.Serialization;
using Meceqs.TypedHandling.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeceqsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Meceqs core services and returns a builder that allows to add or configure Meceqs features.
        /// </summary>
        public static IMeceqsBuilder AddMeceqs(this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            AddPipeline(services);
            AddReceiving(services);
            AddSending(services);
            AddSerialization(services);
            AddTypedHandling(services);

            return new MeceqsBuilder(services);
        }

        private static void AddPipeline(IServiceCollection services)
        {
            services.TryAddSingleton<IFilterContextFactory, DefaultFilterContextFactory>();
            services.TryAddSingleton<IPipelineProvider, DefaultPipelineProvider>();

            // Every pipeline that should be built must get its own builder.
            services.TryAddTransient<IPipelineBuilder, DefaultPipelineBuilder>();
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
            services.TryAddSingleton<IEnvelopeTypeLoader, DefaultEnvelopeTypeLoader>();
        }

        public static void AddTypedHandling(IServiceCollection services)
        {
            services.TryAddSingleton<IHandleContextFactory, DefaultHandleContextFactory>();
            services.TryAddSingleton<IHandleMethodResolver, DefaultHandleMethodResolver>();
            services.TryAddSingleton<IHandlerInvoker, DefaultHandlerInvoker>();
        }
    }
}