using System;
using Meceqs;
using Meceqs.DependencyInjection;
using Meceqs.Handling;
using Meceqs.Sending;
using Meceqs.Sending.TypedSend;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeceqsServiceCollectionExtensions
    {
        public static IMeceqsBuilder AddMeceqs(this IServiceCollection services, Action<MeceqsOptions> setupAction)
        {
            Check.NotNull(services, nameof(services));

            var options = new MeceqsOptions();

            if (setupAction != null)
            {
                setupAction(options);
            }

            return services.AddMeceqs(options);
        }

        public static IMeceqsBuilder AddMeceqs(this IServiceCollection services, MeceqsOptions options)
        {
            Check.NotNull(services, nameof(services));

            // TODO @cweiss check lifecycles!

            // Core
            services.AddSingleton<IMessageContextFactory, DefaultMessageContextFactory>();
            services.AddSingleton<IEnvelopeTypeLoader, DefaultEnvelopeTypeLoader>();
            services.AddTransient<IEnvelopeFactory, DefaultEnvelopeFactory>();

            // Handling
            services.AddTransient<IEnvelopeHandler, DefaultEnvelopeHandler>();
            services.AddTransient<IHandlerInvoker, DefaultHandlerInvoker>();

            // Sending
            services.AddSingleton<IEnvelopeCorrelator, DefaultEnvelopeCorrelator>();
            services.AddTransient<IMeceqsSender, DefaultMeceqsSender>();
            services.AddTransient<IMessageSendingMediator, DefaultMessageSendingMediator>();

            // Sending/TypedSend
            services.AddSingleton<ISenderFactoryInvoker, DefaultSenderFactoryInvoker>();
            services.AddSingleton<ISenderInvoker, DefaultSenderInvoker>();
            services.AddTransient<ISenderFactory, DefaultSenderFactory>();

            return new MeceqsBuilder(services);
        }
    }
}