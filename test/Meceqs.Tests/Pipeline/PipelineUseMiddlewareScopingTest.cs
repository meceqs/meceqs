using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Pipeline
{
    public class PipelineUseMiddlewareScopingTest
    {
        private class MiddlewareWithServices
        {
            private readonly MiddlewareDelegate _next;
            private readonly IMiddlewareCtorService _ctorService;

            public MiddlewareWithServices(MiddlewareDelegate next, bool isTerminating, IMiddlewareCtorService ctorService)
            {
                _ctorService = ctorService;
                if (!isTerminating)
                    _next = next;
            }

            public Task Invoke(MessageContext context, IMiddlewareInvokeService invokeService)
            {
                _ctorService.InvokeCalled();
                invokeService.InvokeCalled();

                return _next?.Invoke(context) ?? Task.CompletedTask;
            }
        }

        private class CallCounter
        {
            public int InvokeCalled { get; set; }
            public int DisposeCalled { get; set; }

            public override string ToString() => $"{nameof(InvokeCalled)}:{InvokeCalled};{nameof(DisposeCalled)}:{DisposeCalled}";
        }

        private interface IMiddlewareCtorService : IDisposable
        {
            void InvokeCalled();
        }

        private class MiddlewareCtorService : IMiddlewareCtorService
        {
            private readonly CallCounter _callCounter;

            public MiddlewareCtorService(IList<CallCounter> callCounters = null)
            {
                _callCounter = new CallCounter();
                callCounters?.Add(_callCounter);
            }

            public void InvokeCalled() => _callCounter.InvokeCalled++;
            public void Dispose() => _callCounter.DisposeCalled++;
        }

        private interface IMiddlewareInvokeService : IDisposable
        {
            void InvokeCalled();
        }

        private class MiddlewareInvokeService : IMiddlewareInvokeService
        {
            private readonly CallCounter _callCounter;

            public MiddlewareInvokeService(IList<CallCounter> callCounters = null)
            {
                _callCounter = new CallCounter();
                callCounters?.Add(_callCounter);
            }

            public void InvokeCalled() => _callCounter.InvokeCalled++;
            public void Dispose() => _callCounter.DisposeCalled++;
        }

        private MiddlewareDelegate GetPipeline(IServiceProvider serviceProvider)
        {
            var pipeline = new PipelineBuilder(serviceProvider);

            // two middleware components to make sure we can test scoped services.
            pipeline.UseMiddleware<MiddlewareWithServices>(false /* isTerminating */);
            pipeline.UseMiddleware<MiddlewareWithServices>(true /* isTerminating */);

            return pipeline.Build();
        }

        [Fact]
        public async Task Transient_MiddlewareCtorService_should_only_be_created_once_per_middleware()
        {
            var ctorServiceCalls = new List<CallCounter>();

            // app start
            var services = new ServiceCollection();
            services.AddTransient<IMiddlewareCtorService>(_ => new MiddlewareCtorService(ctorServiceCalls));
            services.AddTransient<IMiddlewareInvokeService>(_ => new MiddlewareInvokeService());
            var serviceProvider = services.BuildServiceProvider();
            var pipeline = GetPipeline(serviceProvider);

            // multiple requests
            for (int i = 0; i < 5; i++)
            {
                var context = TestObjects.MessageContext<SimpleMessage>(requestServices: serviceProvider);
                await pipeline(context);
            }

            // app shutdown
            (serviceProvider as IDisposable).Dispose();

            // assertions
            ctorServiceCalls.Count.ShouldBe(2);
            ctorServiceCalls.ShouldAllBe(x => x.InvokeCalled == 5);
            ctorServiceCalls.ShouldAllBe(x => x.DisposeCalled == 1);
        }

        [Fact]
        public async Task Transient_MiddlewareInvokeService_should_be_created_for_every_call()
        {
            var invokeServiceCalls = new List<CallCounter>();

            // app start
            var services = new ServiceCollection();
            services.AddTransient<IMiddlewareCtorService>(_ => new MiddlewareCtorService());
            services.AddTransient<IMiddlewareInvokeService>(_ => new MiddlewareInvokeService(invokeServiceCalls));
            var serviceProvider = services.BuildServiceProvider();
            var pipeline = GetPipeline(serviceProvider);

            // multiple requests
            for (int i = 0; i < 5; i++)
            {
                var context = TestObjects.MessageContext<SimpleMessage>(requestServices: serviceProvider);
                await pipeline(context);
            }

            // app shutdown
            (serviceProvider as IDisposable).Dispose();

            // assertions
            invokeServiceCalls.Count.ShouldBe(10);
            invokeServiceCalls.ShouldAllBe(x => x.InvokeCalled == 1);
            invokeServiceCalls.ShouldAllBe(x => x.DisposeCalled == 1);
        }

        [Fact]
        public async Task Scoped_MiddlewareInvokeService_should_be_created_once_per_scope()
        {
            var invokeServiceCalls = new List<CallCounter>();

            // app start
            var services = new ServiceCollection();
            services.AddTransient<IMiddlewareCtorService>(_ => new MiddlewareCtorService());
            services.AddScoped<IMiddlewareInvokeService>(_ => new MiddlewareInvokeService(invokeServiceCalls));
            var serviceProvider = services.BuildServiceProvider();
            var pipeline = GetPipeline(serviceProvider);

            // multiple requests
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            for (int i = 0; i < 5; i++)
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = TestObjects.MessageContext<SimpleMessage>(requestServices: scope.ServiceProvider);

                    await pipeline(context);
                }
            }

            // app shutdown
            (serviceProvider as IDisposable).Dispose();

            // assertions
            invokeServiceCalls.Count.ShouldBe(5);
            invokeServiceCalls.ShouldAllBe(x => x.InvokeCalled == 2);
            invokeServiceCalls.ShouldAllBe(x => x.DisposeCalled == 1);
        }
    }
}