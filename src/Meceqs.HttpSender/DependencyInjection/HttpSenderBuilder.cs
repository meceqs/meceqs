using System;
using System.Linq;
using System.Reflection;
using Meceqs;
using Meceqs.HttpSender;
using Meceqs.Transport;

namespace Microsoft.Extensions.DependencyInjection
{
    public class HttpSenderBuilder : SendTransportBuilder<HttpSenderBuilder, HttpSenderOptions>
    {
        protected override HttpSenderBuilder Instance => this;

        public IHttpClientBuilder HttpClient { get; }

        public HttpSenderBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName)
        {
            HttpClient = meceqsBuilder.Services.AddHttpClient("Meceqs.HttpSender." + PipelineName);

            ConfigurePipeline(pipeline => pipeline.EndsWith(x => x.RunHttpSender()));
        }

        /// <summary>
        /// Add the given message type to this sender. The relative endpoint path will be resolved
        /// by using the configured <see cref="MessageConvention"/>.
        /// </summary>
        public HttpSenderBuilder AddMessage<TMessage>()
        {
            return AddMessage(typeof(TMessage));
        }

        /// <summary>
        /// Add the given message type to this sender. The relative endpoint path will be resolved
        /// by using the configured <see cref="MessageConvention"/>.
        /// </summary>
        public HttpSenderBuilder AddMessage(Type messageType)
        {
            Guard.NotNull(messageType, nameof(messageType));

            return Configure(options =>
            {
                options.Messages.Add(messageType, null);
            });
        }

        public HttpSenderBuilder AddMessagesFromAssembly<TMessage>(Predicate<Type> filter)
        {
            return AddMessagesFromAssembly(typeof(TMessage).GetTypeInfo().Assembly, filter);
        }

        public HttpSenderBuilder AddMessagesFromAssembly(Assembly assembly, Predicate<Type> filter)
        {
            Guard.NotNull(assembly, nameof(assembly));
            Guard.NotNull(filter, nameof(filter));

            var messages = from type in assembly.GetTypes()
                           where type.GetTypeInfo().IsClass && !type.GetTypeInfo().IsAbstract
                           where filter(type)
                           select type;

            foreach (var message in messages)
            {
                AddMessage(message);
            }

            return Instance;
        }

        public HttpSenderBuilder SetBaseAddress(string baseAddress)
        {
            Guard.NotNullOrWhiteSpace(baseAddress, nameof(baseAddress));

            HttpClient.ConfigureHttpClient(client =>
            {
                // The trailing slash is really important:
                // http://stackoverflow.com/questions/23438416/why-is-httpclient-baseaddress-not-working
                client.BaseAddress = new Uri(baseAddress.TrimEnd('/') + "/");
            });

            return Instance;
        }
    }
}
