using Meceqs;
using Meceqs.Configuration;
using Meceqs.Consuming;
using Meceqs.Pipeline;
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

            AddConsuming(services);
            AddPipeline(services);
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

        private static void AddConsuming(IServiceCollection services)
        {
            services.TryAddTransient<IMessageConsumer, MessageConsumer>();
        }

        private static void AddSending(IServiceCollection services)
        {
            services.TryAddSingleton<IEnvelopeFactory, DefaultEnvelopeFactory>();
            services.TryAddSingleton<IEnvelopeCorrelator, DefaultEnvelopeCorrelator>();
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