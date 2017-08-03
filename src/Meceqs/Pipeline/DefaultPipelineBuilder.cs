using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Meceqs.Pipeline
{
    public class DefaultPipelineBuilder : IPipelineBuilder
    {
        private readonly IList<Func<MiddlewareDelegate, MiddlewareDelegate>> _middlewareEntries;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMessageContextEnricher _messageContextEnricher;

        public IServiceProvider ApplicationServices { get; }

        public DefaultPipelineBuilder(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            IMessageContextEnricher messageContextEnricher = null)
        {
            Guard.NotNull(serviceProvider, nameof(serviceProvider));
            Guard.NotNull(loggerFactory, nameof(loggerFactory));

            _middlewareEntries = new List<Func<MiddlewareDelegate, MiddlewareDelegate>>();

            ApplicationServices = serviceProvider;
            _loggerFactory = loggerFactory;
            _messageContextEnricher = messageContextEnricher;
        }

        public IPipelineBuilder Use(Func<MiddlewareDelegate, MiddlewareDelegate> middleware)
        {
            _middlewareEntries.Add(middleware);
            return this;
        }

        public IPipeline Build(string pipelineName)
        {
            Guard.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            MiddlewareDelegate pipeline = context =>
            {
                // This middleware will be executed last!
                throw new InvalidOperationException("The message has not been handled by a terminating middleware");
            };

            foreach (var middleware in _middlewareEntries.Reverse())
            {
                pipeline = middleware(pipeline);
            }

            return new DefaultPipeline(pipeline, pipelineName, _loggerFactory, _messageContextEnricher);
        }
    }
}