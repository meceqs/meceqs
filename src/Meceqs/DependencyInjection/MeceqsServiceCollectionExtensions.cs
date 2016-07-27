using System;
using Meceqs;
using Meceqs.DependencyInjection;
using Meceqs.Handling;
using Meceqs.Sending;

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
            services.AddTransient<IEnvelopeFactory, DefaultEnvelopeFactory>();
            services.AddSingleton<IEnvelopeTypeLoader, DefaultEnvelopeTypeLoader>();

            // Handling
            services.AddTransient<IEnvelopeHandler, DefaultEnvelopeHandler>();
            services.AddTransient<IHandlerInvoker, DefaultHandlerInvoker>();

            // Sending
            services.AddSingleton<IEnvelopeCorrelator, DefaultEnvelopeCorrelator>();
            services.AddTransient<IMeceqsSender, DefaultMeceqsSender>();
            services.AddTransient<IMessageSendingMediator, DefaultMessageSendingMediator>();

            return new MeceqsBuilder(services);
        }
    }
}