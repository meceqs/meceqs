using System;
using Meceqs.Handling;
using Meceqs.Sending.Transport.TypedSend;

namespace Meceqs.ServiceProviderIntegration
{
    public class ServiceProviderFactory :
        IHandlerFactory,
        ISenderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderFactory(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public IHandler<TMessage, TResult> CreateHandler<TMessage, TResult>() where TMessage : IMessage
        {
            return GetService<IHandler<TMessage, TResult>>();
        }

        public ISender<TMessage, TResult> CreateSender<TMessage, TResult>() where TMessage : IMessage
        {
            return GetService<ISender<TMessage, TResult>>();
        }

        private TService GetService<TService>()
        {
            return (TService)_serviceProvider.GetService(typeof(TService));
        }
    }
}