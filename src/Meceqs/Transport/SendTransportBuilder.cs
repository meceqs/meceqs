using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Reflection;
using Meceqs.Pipeline;

namespace Meceqs.Transport
{
    public abstract class SendTransportBuilder<TSendTransportBuilder, TSendTransportOptions> : TransportPipelineBuilder
        where TSendTransportBuilder : SendTransportBuilder<TSendTransportBuilder, TSendTransportOptions>
        where TSendTransportOptions : SendTransportOptions
    {
        /// <summary>
        /// This property is only necessary to support the builder pattern with
        /// the generic arguments from this type.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected abstract TSendTransportBuilder Instance { get; }

        protected SendTransportBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName ?? MeceqsDefaults.SendPipelineName)
        {
            Services.Configure<TSendTransportOptions>(PipelineName, PipelineConfiguration);
        }

        public TSendTransportBuilder Configure(Action<TSendTransportOptions> options)
        {
            if (options != null)
            {
                Services.Configure(PipelineName, options);
            }
            return Instance;
        }

        public TSendTransportBuilder ConfigurePipeline(Action<IPipelineBuilder> pipeline)
        {
            ConfigurePipelineInternal(pipeline);
            return Instance;
        }

        public TSendTransportBuilder AddMessageTypesFromAssembly(Assembly assembly, Func<Type, bool> filter)
        {
            Guard.NotNull(assembly, nameof(assembly));
            Guard.NotNull(filter, nameof(filter));

            foreach (var type in assembly.ExportedTypes)
            {
                if (filter(type))
                {
                    AddMessageType(type);
                }
            }

            return Instance;
        }

        public TSendTransportBuilder AddMessageTypes(params Type[] messageTypes)
        {
            foreach (var messageType in messageTypes)
            {
                AddMessageType(messageType);
            }

            return Instance;
        }

        public TSendTransportBuilder AddMessageType<TMessage>()
        {
            return AddMessageType(typeof(TMessage));
        }

        public TSendTransportBuilder AddMessageType(Type messageType)
        {
            Guard.NotNull(messageType, nameof(messageType));

            Services.Configure<PipelineProviderOptions>(o => o.AddMessageTypeMapping(messageType, PipelineName));
            return Instance;
        }
    }
}
