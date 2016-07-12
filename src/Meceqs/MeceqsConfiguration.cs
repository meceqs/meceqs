using System;
using System.Collections.Generic;
using System.Reflection;
using Meceqs.Handling;
using Meceqs.Sending;
using Meceqs.Sending.Transport;
using Meceqs.ServiceProviderIntegration;

namespace Meceqs
{
    public class MeceqsConfiguration
    {
        private readonly ApplicationInfo _applicationInfo;
        private readonly IList<Assembly> _contractAssemblies = new List<Assembly>();

        public IEnvelopeFactory EnvelopeFactory { get; set; }

        public IEnvelopeTypeLoader EnvelopeTypeLoader { get; set; }

        public IEnvelopeSerializer EnvelopeSerializer { get; set; }

        public IHandlerFactory HandlerFactory { get; set; }

        public IMessageCorrelator MessageCorrelator { get; set; }

        public IMessageSendingMediator MessageSendingMediator { get; set; }

        public ISendTransportFactory SendTransportFactory { get; set; }

        public MeceqsConfiguration(ApplicationInfo applicationInfo)
        {
            Check.NotNull(applicationInfo, nameof(applicationInfo));

            _applicationInfo = applicationInfo;
        }

        public static MeceqsConfiguration Default(IServiceProvider serviceProvider)
        {
            var serviceProviderFactory = new ServiceProviderFactory(serviceProvider);

            // TODO @cweiss change this!
            var applicationInfo = new ApplicationInfo
            {
                ApplicationName = "TODO Change me",
                HostName = "TODO MachineName"
            };

            MeceqsConfiguration defaultConfig = new MeceqsConfiguration(applicationInfo)
                .UseEnvelopeFactory(new DefaultEnvelopeFactory(applicationInfo))
                .UseEnvelopeTypeLoader(new DefaultEnvelopeTypeLoader())
                .UseHandlerFactory(serviceProviderFactory)
                .UseMessageCorrelator(new DefaultMessageCorrelator())
                .UseSendTransportFactory(new DefaultSendTransportFactory(serviceProviderFactory));

            return defaultConfig;
        }

        public IMessageSender CreateMessageSender()
        {
            return new DefaultMessageSender(EnvelopeFactory, MessageCorrelator, MessageSendingMediator);
        }

        public MeceqsConfiguration AddContractAssemblies(params Assembly[] assemblies)
        {
            EnvelopeTypeLoader.AddContractAssemblies(assemblies);
            return this;
        }

        public MeceqsConfiguration UseEnvelopeFactory(IEnvelopeFactory envelopeFactory)
        {
            Check.NotNull(envelopeFactory, nameof(envelopeFactory));

            EnvelopeFactory = envelopeFactory;
            return this;
        }

        public MeceqsConfiguration UseEnvelopeTypeLoader(IEnvelopeTypeLoader envelopeTypeLoader)
        {
            Check.NotNull(envelopeTypeLoader, nameof(envelopeTypeLoader));

            EnvelopeTypeLoader = envelopeTypeLoader;
            return this;
        }

        public MeceqsConfiguration UseEnvelopeSerializer(IEnvelopeSerializer envelopeSerializer)
        {
            Check.NotNull(envelopeSerializer, nameof(envelopeSerializer));

            EnvelopeSerializer = envelopeSerializer;
            return this;
        }

        public MeceqsConfiguration UseHandlerFactory(IHandlerFactory handlerFactory)
        {
            Check.NotNull(handlerFactory, nameof(handlerFactory));

            HandlerFactory = handlerFactory;
            return this;
        }

        public MeceqsConfiguration UseMessageCorrelator(IMessageCorrelator messageCorrelator)
        {
            Check.NotNull(messageCorrelator, nameof(messageCorrelator));

            MessageCorrelator = messageCorrelator;
            return this;
        }

        public MeceqsConfiguration UseSendTransportFactory(ISendTransportFactory sendTransportFactory)
        {
            Check.NotNull(sendTransportFactory, nameof(sendTransportFactory));

            SendTransportFactory = sendTransportFactory;
            return this;
        }
    }
}