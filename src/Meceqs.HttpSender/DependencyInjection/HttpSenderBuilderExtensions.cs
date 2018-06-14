using System;
using System.Linq;
using System.Reflection;
using Meceqs;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpSenderBuilderExtensions
    {
        /// <summary>
        /// Add the given message type to this sender. The relative endpoint path will be resolved
        /// by using the configured <see cref="MessageConvention"/>.
        /// </summary>
        public static IHttpSenderBuilder AddMessage<TMessage>(this IHttpSenderBuilder builder)
        {
            return AddMessage(builder, typeof(TMessage));
        }

        /// <summary>
        /// Add the given message type to this sender. The relative endpoint path will be resolved
        /// by using the configured <see cref="MessageConvention"/>.
        /// </summary>
        public static IHttpSenderBuilder AddMessage(this IHttpSenderBuilder builder, Type messageType)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(messageType, nameof(messageType));

            builder.Configure(options =>
            {
                options.Messages.Add(messageType, null);
            });
            
            return builder;
        }

        public static IHttpSenderBuilder AddMessagesFromAssembly<TMessage>(this IHttpSenderBuilder builder, Predicate<Type> filter)
        {
            return AddMessagesFromAssembly(builder, typeof(TMessage).GetTypeInfo().Assembly, filter);
        }

        public static IHttpSenderBuilder AddMessagesFromAssembly(this IHttpSenderBuilder builder, Assembly assembly, Predicate<Type> filter)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(assembly, nameof(assembly));
            Guard.NotNull(filter, nameof(filter));

            var messages = from type in assembly.GetTypes()
                           where type.GetTypeInfo().IsClass && !type.GetTypeInfo().IsAbstract
                           where filter(type)
                           select type;

            foreach (var message in messages)
            {
                AddMessage(builder, message);
            }

            return builder;
        }

        public static IHttpSenderBuilder SetBaseAddress(this IHttpSenderBuilder builder, string baseAddress)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNullOrWhiteSpace(baseAddress, nameof(baseAddress));

            builder.HttpClient.ConfigureHttpClient(client =>
            {
                // The trailing slash is really important:
                // http://stackoverflow.com/questions/23438416/why-is-httpclient-baseaddress-not-working
                client.BaseAddress = new Uri(baseAddress.TrimEnd('/') + "/");
            });

            return builder;
        }
    }
}
