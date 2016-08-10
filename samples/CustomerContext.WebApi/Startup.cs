using System.Reflection;
using CustomerContext.Contracts.Commands;
using CustomerContext.Contracts.Queries;
using CustomerContext.Core.CommandHandlers;
using CustomerContext.Core.QueryHandlers;
using CustomerContext.Core.Repositories;
using CustomerContext.WebApi.Infrastructure;
using Meceqs.Filters.TypedHandling;
using Meceqs.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Model;

namespace CustomerContext.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // CustomerContext.Core
            services.AddTransient<IHandles<CreateCustomerCommand, CreateCustomerResult>, CustomerCommandHandler>();
            services.AddTransient<IHandles<FindCustomersQuery, FindCustomersResult>, CustomerQueryHandler>();
            services.AddTransient<IHandles<GetCustomerQuery, CustomerDto>, CustomerQueryHandler>();
            services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();

            // Meceqs
            services.AddMeceqs()
                .AddAspNetCore()
                .AddTypedHandling()
                .AddConsumer(options =>
                {
                    var builder = options.Channel.PipelineBuilder;

                    // make sure, MessageType, MessageName, etc are correctly set
                    builder.UseEnvelopeSanitizer();

                    // attach CancellationToken, RequestServices, MessageHistory
                    builder.UseAspNetCore();

                    // builder.UseAuthorization();
                    // builder.UseDataAnnotationsValidator();

                    // forward to IHandles<TMessage, TResult>
                    builder.UseTypedHandling();
                })
                .AddSender(options =>
                {
                    var builder = options.Channel.PipelineBuilder;

                    builder.UseAspNetCore();

                    builder.UseTypedHandling();
                });

            services.AddMvc(options =>
            {
                options.Filters.Add(new RejectInvalidModelStateActionFilter());
                options.Conventions.Add(new BindComplexTypeFromBodyConvention());
            });

            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = Assembly.GetEntryAssembly().GetName().Name
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            app.UseDeveloperExceptionPage();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUi();
        }
    }
}