# Typed Handling Middleware

Meceqs can be used to do simple in-process dispatching to strongly typed handlers
that is similar to Jimmy Bogard's [MediatR](https://github.com/jbogard/MediatR) library.
His library has been a great inspiration for us and we would like to thank him and his contributors for creating it!

Our typed handling middleware has the following features:
* `IHandles<TMessage>` interface for handlers that don't return responses.
* `IHandles<TMessage, TResponse>` interface for handlers that return responses.
* `HandleContext` object that contains metadata (envelope, message context, handler reflection data).
* `IHandleInterceptor` interface for creating interceptors that can read attributes from the handler method/class.
* Handlers and interceptors can be configured separately for every pipeline.
* Handlers and interceptors are resolved transiently by default but you can also use your own lifecycles by
    adding them to the DI framework.
* The middleware can either ignore messages without a handler or throw an exception.

Note that it currently does *not* support the following things:
* Dispatching one message to multiple handlers

## Handlers

Your handler has to implement one of these interfaces:

```csharp
using Meceqs.TypedHandling;

// Implement this interface if your handler does not return a response.
public interface IHandles<TMessage>
{
    Task HandleAsync(TMessage message, HandleContext context);
}

// Implement this interface if your handler returns a response.
public interface IHandles<TMessage, TResponse>
{
    Task<TResponse> HandleAsync(TMessage message, HandleContext context);
}
```

An example for a handler that handles two messages can be seen here:

```csharp
public class CustomerCommandHandler :
    IHandles<CreateCustomerCommand, CreateCustomerResponse>,
    IHandles<ChangeNameCommand>
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [CustomLogic /* this attribute can be read by an IHandleInterceptor */]
    public async Task<CreateCustomerResponse> HandleAsync(CreateCustomerCommand cmd, HandleContext context)
    {
        var customer = new Customer(cmd.FirstName, cmd.LastName);

        _customerRepository.Add(customer);

        return new CreateCustomerResponse { CustomerId = customer.Id };
    }

    public async Task HandleAsync(ChangeNameCommand cmd, HandleContext context)
    {
        var customer = _customerRepository.GetById(cmd.CustomerId);

        if (customer == null)
            throw new InvalidOperationException("customer not found");

        customer.ChangeName(cmd.FirstName, cmd.LastName);
    }
}
```

## Interceptors

In contrast to regular middleware, interceptors have access to the `HandleContext`. Since this context contains metadata
about the handle method and class, interceptors can read custom attributes from the method/class to implement attribute-based
aspect oriented programming concepts like authorization, transaction handling etc.

The following interceptor will use `HandleContext.HandleMethod` and `HandleContext.HandlerType` to look for a
`CustomLogicAttribute`.

```csharp
public class SampleHandleInterceptor : IHandleInterceptor
{
    private readonly ILogger _logger;

    public SampleHandleInterceptor(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<SampleHandleInterceptor>();
    }

    public Task OnHandleExecutionAsync(HandleContext context, HandleExecutionDelegate next)
    {
        var customAttribute = context.HandleMethod.GetCustomAttribute(typeof(CustomLogicAttribute));
        if (customAttribute != null)
        {
            _logger.LogInformation("Custom attribute found on method");
        }

        customAttribute = context.HandlerType.GetCustomAttribute(typeof(CustomLogicAttribute));
        if (customAttribute != null)
        {
            _logger.LogInformation("Custom attribute found on class");
        }

        return next(context);
    }
}
```

## Configuration

You can add the typed handling middleware to any pipeline by calling the following code in your configuration code:

```csharp
services.AddMeceqs()
    .AddPipeline("my-pipeline", pipeline =>
    {
        //pipeline.UseYourCustomMiddleware();

        // This must be the last middleware in the pipeline because it is terminal.
        pipeline.RunTypedHandling(options =>
        {
            // This will tell the middleware to add all handlers from the assembly of the given type.
            options.Handlers.AddFromAssembly<CustomerCommandHandler>();

            // This would add only the given handler.
            //options.Handlers.Add<CustomerCommandHandler>();

            // This interceptor is created for each message.
            // (It doesn't need to be registered in the DI framework
            // because Meceqs uses ActivatorUtilities to create the instance.)
            options.Interceptors.Add<SampleHandleInterceptor>();

            // This interceptor will use the lifecycle from the DI framework.
            options.Interceptors.AddService<SingletonHandleInterceptor>();

            // This will tell the middleware to throw if there's no handler for a message.
            // (this is the default behavior)
            options.ThrowOnUnknownMessage();

            // This would tell the middleware to ignore messages without a handler.
            //options.SkipUnknownMessages();
        });
    });

// Meceqs resolves interceptors transiently by default.
// To change this, you can add the interceptor with your own lifecycle
// to the DI framework and use "Interceptors.AddService()" to
// tell Meceqs to resolve it from there.
services.AddSingleton<SingletonHandleInterceptor>();
```
