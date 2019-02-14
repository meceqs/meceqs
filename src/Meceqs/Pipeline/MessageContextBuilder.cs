using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Pipeline
{
    public abstract class MessageContextBuilder<TBuilder> : IMessageContextBuilder<TBuilder>
        where TBuilder : IMessageContextBuilder<TBuilder>
    {
        protected string DefaultPipelineName { get; }
        protected Envelope Envelope { get; }
        protected IServiceProvider RequestServices { get; }

        protected CancellationToken Cancellation { get; private set; }
        protected MessageContextItems ContextItems { get; private set; }
        protected ClaimsPrincipal User { get; private set; }
        protected string ForcedPipelineName { get; private set; }

        /// <summary>
        /// Returning "this" in methods is not possible because "TBuilder" is not a derived type from this.
        /// </summary>
        public abstract TBuilder Instance { get; }

        protected MessageContextBuilder(string defaultPipelineName, Envelope envelope, IServiceProvider serviceProvider)
        {
            Guard.NotNullOrWhiteSpace(defaultPipelineName, nameof(defaultPipelineName));
            Guard.NotNull(envelope, nameof(envelope));
            Guard.NotNull(serviceProvider, nameof(serviceProvider));

            DefaultPipelineName = defaultPipelineName;
            Envelope = envelope;
            RequestServices = serviceProvider;
        }

        public TBuilder SetCancellationToken(CancellationToken cancellation)
        {
            Cancellation = cancellation;
            return Instance;
        }

        public TBuilder SetContextItem(string key, object value)
        {
            if (ContextItems == null)
            {
                ContextItems = new MessageContextItems();
            }

            ContextItems.Set(key, value);
            return Instance;
        }

        public TBuilder SetUser(ClaimsPrincipal user)
        {
            User = user;
            return Instance;
        }

        public TBuilder UsePipeline(string pipelineName)
        {
            Guard.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            ForcedPipelineName = pipelineName;
            return Instance;
        }

        public TBuilder SetHeader(string headerName, object value)
        {
            Envelope.Headers[headerName] = value;
            return Instance;
        }

        protected virtual MessageContext CreateMessageContext(string pipelineName, Envelope envelope, Type responseType)
        {
            Guard.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));
            Guard.NotNull(envelope, nameof(envelope));

            var context = new MessageContext(envelope, pipelineName, RequestServices, responseType ?? typeof(void));

            context.Cancellation = Cancellation;
            context.User = User;

            if (ContextItems != null)
            {
                context.Items.Add(ContextItems);
            }

            return context;
        }

        private IPipeline GetPipeline()
        {
            var pipelineProvider = RequestServices.GetRequiredService<IPipelineProvider>();
            return pipelineProvider.GetPipeline(Envelope.Message.GetType(), ForcedPipelineName, DefaultPipelineName);
        }

        protected async Task InvokePipelineAsync()
        {
            var pipeline = GetPipeline();

            var messageContext = CreateMessageContext(pipeline.Name, Envelope, responseType: typeof(void));

            await pipeline.InvokeAsync(messageContext);
        }

        protected async Task<TResponse> InvokePipelineAsync<TResponse>()
        {
            var pipeline = GetPipeline();

            var messageContext = CreateMessageContext(pipeline.Name, Envelope, typeof(TResponse));

            await pipeline.InvokeAsync(messageContext);

            return (TResponse)messageContext.Response;
        }

        protected async Task<object> InvokePipelineAsync(Type responseType)
        {
            var pipeline = GetPipeline();

            var messageContext = CreateMessageContext(pipeline.Name, Envelope, responseType);

            await pipeline.InvokeAsync(messageContext);

            return messageContext.Response;
        }
    }
}
