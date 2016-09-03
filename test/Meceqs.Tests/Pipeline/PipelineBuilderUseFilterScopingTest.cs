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
    public class PipelineBuilderUseFilterScopingTest
    {
        private class FilterWithServices
        {
            private readonly FilterDelegate _next;
            private readonly IFilterCtorService _ctorService;

            public FilterWithServices(FilterDelegate next, bool isTerminating, IFilterCtorService ctorService)
            {
                _ctorService = ctorService;
                if (!isTerminating) 
                    _next = next;
            }

            public Task Invoke(FilterContext context, IFilterInvokeService invokeService)
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

        private interface IFilterCtorService : IDisposable
        {
            void InvokeCalled();
        }

        private class FilterCtorService : IFilterCtorService
        {
            private readonly CallCounter _callCounter;

            public FilterCtorService(IList<CallCounter> callCounters = null)
            {
                _callCounter = new CallCounter();
                callCounters?.Add(_callCounter);
            }

            public void InvokeCalled() => _callCounter.InvokeCalled++;
            public void Dispose() => _callCounter.DisposeCalled++;
        }

        private interface IFilterInvokeService : IDisposable
        {
            void InvokeCalled();
        }

        private class FilterInvokeService : IFilterInvokeService
        {
            private readonly CallCounter _callCounter;

            public FilterInvokeService(IList<CallCounter> callCounters = null)
            {
                _callCounter = new CallCounter();
                callCounters?.Add(_callCounter);
            }

            public void InvokeCalled() => _callCounter.InvokeCalled++;
            public void Dispose() => _callCounter.DisposeCalled++;
        }

        private IPipeline GetPipeline(IServiceProvider serviceProvider)
        {
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var builder = new DefaultPipelineBuilder("pipeline", serviceProvider, loggerFactory);
            
            // two filters to make sure we can test scoped services.
            builder.UseFilter<FilterWithServices>(false /* isTerminating */);
            builder.UseFilter<FilterWithServices>(true /* isTerminating */);
            
            return builder.Build();
        }

        [Fact]
        public async Task Transient_FilterCtorService_should_only_be_created_once_per_filter()
        {
            var ctorServiceCalls = new List<CallCounter>();
            
            // app start
            var services = new ServiceCollection();
            services.AddTransient<IFilterCtorService>(_ => new FilterCtorService(ctorServiceCalls));
            services.AddTransient<IFilterInvokeService>(_ => new FilterInvokeService());
            var serviceProvider = services.BuildServiceProvider();
            var pipeline = GetPipeline(serviceProvider);

            // multiple requests
            for (int i = 0; i < 5; i++)
            {
                var context = TestObjects.FilterContext<SimpleMessage>();
                await pipeline.ProcessAsync(context);
            }

            // app shutdown
            (serviceProvider as IDisposable).Dispose();

            // assertions
            ctorServiceCalls.Count.ShouldBe(2);
            ctorServiceCalls.ShouldAllBe(x => x.InvokeCalled == 5);
            ctorServiceCalls.ShouldAllBe(x => x.DisposeCalled == 1);
        }

        [Fact]
        public async Task Transient_FilterInvokeService_should_be_created_for_every_call()
        {
            var invokeServiceCalls = new List<CallCounter>();

            // app start
            var services = new ServiceCollection();
            services.AddTransient<IFilterCtorService>(_ => new FilterCtorService());
            services.AddTransient<IFilterInvokeService>(_ => new FilterInvokeService(invokeServiceCalls));
            var serviceProvider = services.BuildServiceProvider();
            var pipeline = GetPipeline(serviceProvider);

            // multiple requests
            for (int i = 0; i < 5; i++)
            {
                var context = TestObjects.FilterContext<SimpleMessage>();
                await pipeline.ProcessAsync(context);
            }

            // app shutdown
            (serviceProvider as IDisposable).Dispose();

            // assertions
            invokeServiceCalls.Count.ShouldBe(10);
            invokeServiceCalls.ShouldAllBe(x => x.InvokeCalled == 1);
            invokeServiceCalls.ShouldAllBe(x => x.DisposeCalled == 1);
        }

        [Fact]
        public async Task Scoped_FilterInvokeService_should_be_created_once_per_scope()
        {
            var invokeServiceCalls = new List<CallCounter>();

            // app start
            var services = new ServiceCollection();
            services.AddTransient<IFilterCtorService>(_ => new FilterCtorService());
            services.AddScoped<IFilterInvokeService>(_ => new FilterInvokeService(invokeServiceCalls));
            var serviceProvider = services.BuildServiceProvider();
            var pipeline = GetPipeline(serviceProvider);

            // multiple requests
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            for (int i = 0; i < 5; i++)
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var context = TestObjects.FilterContext<SimpleMessage>();
                    context.RequestServices = scope.ServiceProvider;

                    await pipeline.ProcessAsync(context);
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