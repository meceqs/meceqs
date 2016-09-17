# Typed Handling Filter

Meceqs can be used to do simple in-process dispatching that is similar to Jimmy Bogard's [MediatR](https://github.com/jbogard/MediatR) library.
His library has been a great inspiration for us and we would like to thank him and his contributors for creating it!

Basically, this filter allows you to write strongly typed handler classes for your message types. 

Our typed handling filter has the following features:
* `IHandles<TMessage>` interface for handlers that don't return results.
* `IHandles<TMessage, TResult>` interface for handlers that return results.
* `HandleContext` object that contains metadata (envelope, filter context, handler reflection data).
* `IHandleInterceptor` interface for creating interceptors that can read attributes from the handler method/class. 
* Handlers and interceptors can be configured separately for every pipeline.
* Handlers and interceptors are resolved transiently by default but you can also use your own lifecycles by 
    adding them to the DI framework.
* The filter can either ignore messages without a handler or throw an exception.

## Handlers

Your handler has to implement one of these interfaces:

```csharp
using Meceqs.TypedHandling;

// Implement this interface if your handler does not return a result.
public interface IHandles<TMessage>
{
    Task HandleAsync(HandleContext<TMessage> message);
}

// Implement this interface if your handler returns a result.
public interface IHandles<TMessage, TResult>
{
    Task<TResult> HandleAsync(HandleContext<TMessage> message);
}
```

An example for a handler that handles two messages can be seen here:

```csharp
public class CustomerCommandHandler :
    IHandles<CreateCustomerCommand, CreateCustomerResult>,
    IHandles<ChangeNameCommand>
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [CustomLogic /* this attribute can be read by an IHandleInterceptor */]
    public async Task<CreateCustomerResult> HandleAsync(HandleContext<CreateCustomerCommand> context)
    {
        var cmd = context.Message;

        var customer = new Customer(cmd.FirstName, cmd.LastName);

        _customerRepository.Add(customer);

        return new CreateCustomerResult { CustomerId = customer.Id };
    }

    public async Task HandleAsync(HandleContext<ChangeNameCommand> context)
    {
        var cmd = context.Message;

        var customer = _customerRepository.GetById(cmd.CustomerId);

        if (customer == null)
            throw new InvalidOperationException("customer not found");

        customer.ChangeName(cmd.FirstName, cmd.LastName);
    }
}
```

## Interceptors

In contrast to regular filters, interceptors have access to the `HandleContext`. Since this context contains metadata 
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
        OnHandleExecuting(context);

        return next(context);
    }

    private void OnHandleExecuting(HandleContext context)
    {
        var customAttribute = context.HandleMethod.CustomAttributes
            .FirstOrDefault(x => x.AttributeType == typeof(CustomLogicAttribute));

        if (customAttribute != null)
        {
            _logger.LogInformation("Custom attribute found on method");
        }

        customAttribute = context.HandlerType.GetTypeInfo().CustomAttributes
            .FirstOrDefault(x => x.AttributeType == typeof(CustomLogicAttribute));

        if (customAttribute != null)
        {
            _logger.LogInformation("Custom attribute found on class");
        }
    }
}
```

## Configuration

You can add the typed handling filter to any pipeline by calling the following code in your configuration code:

```csharp
services.AddMeceqs()
    .AddPipeline("my-pipeline", pipeline =>
    {
        //pipeline.UseYourCustomFilter();

        // This must be the last filter in the pipeline because it is terminal.
        pipeline.RunTypedHandling(options =>
        {
            // This will tell the filter to add all handlers from the assembly of the given type.
            options.Handlers.AddFromAssembly<CustomerCommandHandler>();

            // This would add only the given handler.
            //options.Handlers.Add<CustomerCommandHandler>();

            // This interceptor is created for each message.
            // (It doesn't need to be registered in the DI framework
            // because Meceqs uses ActivatorUtilities to create the instance.)
            options.Interceptors.Add<SampleHandleInterceptor>();

            // This interceptor will use the lifecycle from the DI framework.
            options.Interceptors.AddService<SingletonHandleInterceptor>();

            // This will tell the filter to throw if there's no handler for a message.
            // (this is the default behavior)
            options.ThrowOnUnknownMessage();

            // This would tell the filter to ignore messages without a handler.
            //options.SkipUnknownMessages();
        });
    });

// Meceqs resolves interceptors transiently by default.
// To change this, you can add the interceptor with your own lifecycle
// to the DI framework and use "Interceptors.AddService()" to
// tell Meceqs to resolve it from there.
services.AddSingleton<SingletonHandleInterceptor>();
```