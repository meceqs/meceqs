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
        protected Envelope Envelope { get; }

        protected CancellationToken Cancellation { get; private set; }
        protected MessageContextItems ContextItems { get; private set; }
        protected string PipelineName { get; private set; }
        protected IServiceProvider RequestServices { get; private set; }
        protected ClaimsPrincipal User { get; private set; }

        /// <summary>
        /// Returning "this" in methods is not possible because "TBuilder" is not a derived type from this.
        /// </summary>
        public abstract TBuilder Instance { get; }

        protected MessageContextBuilder(string defaultPipelineName, Envelope envelope, IServiceProvider serviceProvider)
        {
            Guard.NotNullOrWhiteSpace(defaultPipelineName, nameof(defaultPipelineName));
            Guard.NotNull(envelope, nameof(envelope));
            Guard.NotNull(serviceProvider, nameof(serviceProvider));

            PipelineName = defaultPipelineName;
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

        public TBuilder SetRequestServices(IServiceProvider requestServices)
        {
            Guard.NotNull(requestServices, nameof(requestServices));

            RequestServices = requestServices;
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

            PipelineName = pipelineName;
            return Instance;
        }

        public TBuilder SetHeader(string headerName, object value)
        {
            Envelope.Headers[headerName] = value;
            return Instance;
        }

        protected virtual MessageContext CreateMessageContext(Envelope envelope, Type resultType)
        {
            Guard.NotNull(envelope, nameof(envelope));

            var context = new MessageContext(envelope, PipelineName, RequestServices, resultType ?? typeof(void));

            context.Cancellation = Cancellation;
            context.User = User;

            if (ContextItems != null)
            {
                context.Items.Add(ContextItems);
            }

            return context;
        }

        protected async Task InvokePipelineAsync()
        {
            var pipelineProvider = RequestServices.GetRequiredService<IPipelineProvider>();
            var pipeline = pipelineProvider.GetPipeline(PipelineName);

            var messageContext = CreateMessageContext(Envelope, resultType: typeof(void));

            await pipeline.InvokeAsync(messageContext);
        }

        protected async Task<TResult> InvokePipelineAsync<TResult>()
        {
            var pipelineProvider = RequestServices.GetRequiredService<IPipelineProvider>();
            var pipeline = pipelineProvider.GetPipeline(PipelineName);

            var messageContext = CreateMessageContext(Envelope, typeof(TResult));

            await pipeline.InvokeAsync(messageContext);

            return (TResult)messageContext.Result;
        }

        protected async Task<object> InvokePipelineAsync(Type resultType)
        {
            var pipelineProvider = RequestServices.GetRequiredService<IPipelineProvider>();
            var pipeline = pipelineProvider.GetPipeline(PipelineName);

            var messageContext = CreateMessageContext(Envelope, resultType);

            await pipeline.InvokeAsync(messageContext);

            return messageContext.Result;
        }
    }
}