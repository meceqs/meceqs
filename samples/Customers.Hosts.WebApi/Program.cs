using System.Security.Claims;
using System.Threading.Tasks;
using Customers.Core.CommandHandlers;
using Customers.Core.Repositories;
using idunno.Authentication.Basic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SampleConfig;

namespace Customers.Hosts.WebApi
{
    public class Program
    {
        private const string ApiVersion = "v1";
        private const string ApiName = "Customers API";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    // Customers.Core
                    services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();

                    // Incoming requests must be authenticated.
                    // Uses basic auth FOR DEMO PURPOSES!!! (https://github.com/blowdart/idunno.Authentication)
                    // Use e.g. AddJwtBearer() with OAuth tokens instead!
                    services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                        .AddBasic(options =>
                        {
                            options.Realm = "meceqs-sample";
                            options.AllowInsecureProtocol = true; // For demo purposes only, of course!
                            options.Events = new BasicAuthenticationEvents
                            {
                                OnValidateCredentials = context =>
                                {
                                    if (context.Username == "username" && context.Password == "password")
                                    {
                                        context.Principal = new ClaimsPrincipal(
                                            new ClaimsIdentity(new [] {
                                                new Claim(ClaimTypes.NameIdentifier, context.Username),
                                            },
                                            BasicAuthenticationDefaults.AuthenticationScheme));

                                        context.Success();
                                    }
                                    return Task.CompletedTask;
                                }
                            };
                        });

                    // MVC (Only required if you need your own controllers/pages)
                    services.AddMvc();

                    services.AddSwaggerGen(options =>
                    {
                        options.SwaggerDoc(ApiVersion, new OpenApiInfo
                        {
                            Title = ApiName,
                            Version = ApiVersion
                        });

                        options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "basic"
                        });

                        // This registers all handlers that have been added to the AspNetCoreReceiver
                        options.AddMeceqs(x => x.SecurityDefinition = "basic");
                    });
                })
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.UseUrls(SampleConfiguration.CustomersWebApiUrl);
                    webHostBuilder.ConfigureMeceqs((hostingContext, meceqs) =>
                    {
                        // Meceqs resolves interceptors transiently by default.
                        // To change this, you can add the interceptor with your own lifecycle
                        // to the DI framework and use "Interceptors.AddService()" to
                        // tell Meceqs to resolve it from there.
                        meceqs.Services.AddSingleton<SingletonHandleInterceptor>();

                        meceqs
                            // The Web API will process incoming requests
                            .AddAspNetCoreReceiver(receiver =>
                            {
                                // Add a "/v1/" prefix to all message routes
                                receiver.SetRoutePrefix("/" + ApiVersion);

                                // This adds a custom middleware to the pipeline.
                                // They are executed in this order before the TypedHandling middleware is executed.
                                receiver.ConfigurePipeline(pipeline =>
                                {
                                    pipeline.UseAuditing(); // add user id to message if not present
                                });

                                // Process messages with `IHandles<...>`-implementations.
                                receiver.UseTypedHandling(options =>
                                {
                                    // In this example, the context only handles messages from this Web API
                                    // so we can just add every handler.
                                    options.Handlers.AddFromAssembly<CustomerCommandHandler>();

                                    // Interceptors know about the executing handler
                                    // (e.g. to check for attributes on the handler)

                                    // This interceptor is created for each message.
                                    // (It doesn't need to be registered in the DI framework
                                    // because Meceqs uses ActivatorUtilities to create the instance.)
                                    options.Interceptors.Add<SampleHandleInterceptor>();

                                    // this interceptor will use the lifecycle from the DI framework.
                                    options.Interceptors.AddService<SingletonHandleInterceptor>();
                                });
                            })

                            // This Web API will also send messages to an Azure Event Hub.
                            .AddEventHubSender(sender =>
                            {
                                sender.ConfigurePipeline(pipeline =>
                                {
                                    pipeline.UseAuditing(); // add user id to message if not present
                                });

                                if (hostingContext.HostingEnvironment.IsDevelopment())
                                {
                                    // For this sample, we will send messages to a local file instead of a real Event Hub.
                                    sender.UseFileFake(SampleConfiguration.FileFakeEventHubDirectory, "customers");
                                }
                            });
                    });
                    webHostBuilder.Configure(app =>
                    {
                        app.UseDeveloperExceptionPage();

                        app.UseRouting();

                        app.UseAuthentication();
                        app.UseAuthorization();

                        app.UseMeceqs();

                        app.UseEndpoints(endpoints =>
                        {
                            // MVC (Only required if you need your own controllers/pages)
                            endpoints.MapControllers();
                        });

                        // Swagger
                        app.UseSwagger(options =>
                        {
                            options.RouteTemplate = "swagger/{documentName}/swagger.json";
                        });
                        app.UseSwaggerUI(options =>
                        {
                            options.SwaggerEndpoint($"{ApiVersion}/swagger.json", $"{ApiName} - {ApiVersion}");
                        });
                    });
                });
        }
    }
}
