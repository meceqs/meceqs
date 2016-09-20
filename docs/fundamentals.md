# Fundamentals

The following concepts build the foundation of Meceqs:

* __Message:__
    Represents a command/query/event that holds your business data.
* __Envelope:__
    Every *message* is wrapped in an *envelope* that holds potential message headers and important metadata for correlation, de-duplication, diagnostics, serialization.
* __Pipeline:__
    *Envelopes* are sent to a *pipeline*. A pipeline consists of one or many *filters* that can modify or process the *envelope*.
    An application can have multiple pipelines (e.g. for doing HTTP calls, for sending messages to a message broker or for consuming messages from a broker).
* __Filter:__
    A *filter* is one processing unit in a *pipeline*. Filters are similar to "middleware" in ASP.NET Core.
    A filter can have an intermediary role (e.g. log/modify/enrich the *envelope*) and call into the next *filter* or
    it can process the envelope (e.g. send it to an external messaging system) and terminate the pipeline.
* __FilterContext:__
    A context object that is passed from filter to filter. It holds the envelope and additional data like cancellation tokens,
    the current user etc. This is similar to the `HttpContext` type from ASP.NET Core.
* __Result objects:__
    For synchronous request/response messaging, the pipeline can be invoked with an expected result type
    and any *filter* can set the `FilterContext.Result` property to return the result object to the caller.
    This is similar to the `HttpContext.Response` feature in ASP.NET Core.

## Messages

Meceqs does not require your messages to implement certain interfaces (e.g. IMessage etc).
This means, you can define your messages in a library that does not need to reference a Meceqs library.

We recommend that you follow the CQS pattern and separate messages into *commands*, *queries* and *events*:

```csharp
// A command represents an action that should happen and results in a state change.
public class CreateCustomerCommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

// An event represents something that has happened.
public class CustomerCreatedEvent
{
    public Guid CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

// A query returns data and does not change state.
public class GetCustomerQuery
{
    public Guid CustomerId { get; set; }
}

// Result objects can be of any type.
public class GetCustomerResult
{
    public Guid CustomerId { get; set; }
    public string FullName { get; set; }
}
```

## Invoking a pipeline - the long way

To help you better understand Meceqs, this chapter will show you the necessary steps to invoke a pipeline without a higher-level abstraction.
The following chapter will then show you the easy way!

### Envelopes

The first step to invoke a pipeline is to wrap the message in an `Envelope`:

```csharp
CreateCustomerCommand cmd = new CreateCustomerCommand { FirstName = "John", LastName = "Snow" };

Envelope envelope = new Envelope<CreateCustomerCommand>(message: cmd, messageId: Guid.NewGuid());

// You can add headers for information that shouldn't/can't be a regular property in your message.
envelope.Headers.Add("createdBy", "admin");
```

Envelopes contain metadata about the message. This json serialized object shows the properties of an envelope:
```json
{
  // The actual message you've created
  "message": {
    "firstName": "John",
    "lastName": "Snow"
  },

  // A unique id for every message
  "messageId": "49f32326-a4a3-4242-9d8f-396c35db2f67",

  // The unique type of your message - usually used for deserialization.
  "messageType": "Customers.Contracts.Commands.CreateCustomerCommand",

  // A shorter, preferably unique, name - usually used for batch/stream processing where you don't need the full type.
  "messageName": "CreateCustomerCommand",

  // All messages in a process/conversation will have the same correlation id.
  "correlationId": "49f32326-a4a3-4242-9d8f-396c35db2f67",

  // The time at which the envelope has been created.
  "createdOnUtc": "2016-09-17T19:48:28.5447192Z",

  // Key:value pairs that are not properties in your message but should be stored with it anyway.
  "headers": {
    "createdBy": "admin"
  },

  // A list of all components that processed this envelope.
  "history": [ ]
}
```

### FilterContext

Filters in a pipeline may need to forward data to other filters, access the current user, resolve services from the DI container, etc.
To make sure this is possible and extensible, a `FilterContext` will be passed from filter to filter. This object contains your envelope
and many additional properties.

```csharp
FilterContext filterContext = new FilterContext<CreateCustomerCommand>(envelope);

// These values will usually come from your framework (e.g. from HttpContext in ASP.NET Core).
filterContext.Cancellation = _cancellationToken;
filterContext.RequestServices = _serviceProvider;
filterContext.User = _user;

// A filter might execute some logic based on this. This will not be stored in the envelope.
filterContext.Items.Set("someFlag", true);
```

### Invoking a pipeline

An application can have multiple pipelines. Pipelines are identified by name.
The following code will retrieve a `IPipeline` with the name `Send` and invoke it for your message.

```csharp
// The pipeline provider is a singleton instance in your DI container.
IPipelineProvider pipelineProvider;

// A pipeline is identfied by name.
IPipeline pipeline = pipelineProvider.GetPipeline("Send");

// This will invoke the pipeline for your message.
await pipeline.ProcessAsync(filterContext);
```

It's important to understand that invoking a pipeline is a simple in-process method call that
uses `Task` and `async/await` to minimize blocking.
Invoking a pipeline does not start a thread and also doesn't use a queuing system. A pipeline is just
a list of filter objects that are invoked sequentially.

Filters are described shortly, but first you need to see the easy way for invoking a pipeline!

## Invoking a pipeline - the short and easy way

The previous chapter showed every single step necessary to invoke a pipeline for a message.
However, it's not reasonable to write all of this code for every message.
For this reason, Meceqs offers an easy abstraction on top of pipelines.

Typically, a pipeline is used for one of two scenarios:

* __Sending messages:__
    Whenever you *create a new message* in your application, you want to *send* it away.
    This typically happens when your application is a mobile client, a web frontend, etc.
    In this case, the last filter in the pipeline might do one of the following things:
    * Create a synchronous HTTP call for commands/queries
    * Send commands/events to a message broker (e.g. Azure Service Bus, RabbitMQ)
    * Save events in a database/event store (e.g. SQL, GetEventStore, Azure Event Hubs, Apache Kafka)
    * Execute more business logic in case of a decoupled in-process architecture
    * ...
* __Consuming messages:__
    Whenever your application *receives an existing envelope* from somewhere, you want to *consume* it.
    This typically happens when your application is a Web API, a worker process for incoming messages from a broker, etc.
    In this case, the last filter in the pipeline usually does one of the following things:
    * Execute business logic for an incoming command/query/event (and maybe return a result)
    * Create additional messages (by using the same mechanism as in the previously mentioned **Sending messages**)
    * Store/forward the envelope
    * ...

Both scenarios require pipelines with different filters and many applications need both scenarios
so there must be an easy way to know which pipeline should be invoked.
Doing so manually (as in the previous chapter) would require you to create the `FilterContext` yourself
and to specify the name of the pipeline for each call.

To make this easier and obvious, Meceqs includes two interfaces for *sending* and *consuming* messages
that automatically create the filter context and invoke the proper pipeline:

### Meceqs.Sending.IMessageSender
The interface `Meceqs.Sending.IMessageSender` is used for __sending a new message__ or
for __forwarding an existing envelope__ to a pipeline with the default name `Send`.

In our `CreateCustomerCommand` example, the frontend web application would send the command with the following code:

```csharp
CreateCustomerCommand cmd = new CreateCustomerCommand { FirstName = "John", LastName = "Snow" };

await _messageSender.SendAsync(cmd);
```

If the target returns a result for the command, the web application would use this code
(assuming the target returns a `CreateCustomerResult` object with a `CustomerId` property):

```csharp
CreateCustomerCommand cmd = new CreateCustomerCommand { FirstName = "John", LastName = "Snow" };

CreateCustomerResult result = await _messageSender.SendAsync<CreateCustomerResult>(cmd);

Debug.WriteLine("CustomerId: " + result.CustomerId);
```

If you need to set headers on the envelope or some medata on the filter context,
you can use `ForMessage()` which returns a convenient builder object:

```csharp
CreateCustomerCommand cmd = new CreateCustomerCommand { FirstName = "John", LastName = "Snow" };

await _messageSender.ForMessage(cmd)
    .SetHeader("createdBy", "admin")
    .UsePipeline("ServiceBus") // Uses an alternative pipeline if you have more than one.
    .SendAsync();
```

### Meceqs.Consuming.IMessageConsumer
The interface `Meceqs.Consuming.IMessageConsumer` is used for __consuming an existing envelope__
on a pipeline with the default name `Consume`.

```csharp
// The envelope would come from Azure Service Bus, a HTTP call, ...
Envelope existingEnvelope;

// Consume using the shortcut...
await _messageConsumer.ConsumeAsync(existingEnvelope);

// ... or using the builder pattern
await _messageConsumer.ForEnvelope(existingEnvelope)
    .UsePipeline("SomeOtherPipeline")
    .ConsumeAsync();
```

In typical asynchronous cases (like processing messages from a queue, ...) a message consumer does not return a result.
However, if the message consumer is part of a synchronous conversation (like a HTTP call) it's very common
that the consumer has to return a result to the caller - especially in case of query requests.
For this reason, `IMessageConsumer` also offers a `ConsumeAsync<TResult>()` method.

Following up in our example, the ASP.NET Core backend API would implement a MVC controller with the following code
to process the `CreateCustomerCommand` and to immediately return a `CreateCustomerResult`:

```csharp
[HttpPost]
public Task<CreateCustomerResult> CreateCustomer([FromBody] Envelope<CreateCustomerCommand> envelope)
{
    return _messageConsumer.ConsumeAsync<CreateCustomerResult>(envelope);
}
```

This example also shows how easy it is to integrate Meceqs into an ASP.NET Core MVC application.
By specifying the envelope as an action parameter, MVC will automatically handle the deserialization.

## Writing a filter

Once a pipeline is invoked, the filter context will be passed from filter to filter.

*Filters* are very similar to [ASP.NET Core middleware](https://docs.asp.net/en/latest/fundamentals/middleware.html).
It's a simple class that takes a `FilterDelegate next` constructor parameter representing the next filter and that
has a `public Task Invoke(FilterContext context)` method. You have to call `_next(context)` in this method to invoke
the next filter. This means, you can also use `using` and `try..catch` blocks around this call.

Filters support dependency injection on their constructor and on the `Invoke` method. It's important to understand
that __filters are singleton objects__, this means constructor parameters are resolved just once from the root service provider
whereas additional parameters on the `Invoke`-method are resolved for every message.

A simple exception logging filter would be implemented as below:

```csharp
public class ExceptionLoggerFilter
{
    private readonly FilterDelegate _next;
    private readonly ILogger _logger;

    // Filters are singleton objects. Services injected into the constructor
    // are resolved just once from the root service provider.
    public ExceptionLoggerFilter(FilterDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<ExceptionLoggerFilter>();
    }

    // You can add additional parameters to this method to resolve services from
    // the scoped service provider.
    public async Task Invoke(FilterContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(0, ex, "Unhandled exception for {MessageType}", context.MessageType);
            throw;
        }
    }
}
```

## Terminal filters

At the end of every pipeline there must be a terminal filter that doesn't call into a next filter.
This filter will process the message in a way that is useful for your scenario. In case of a sending pipeline,
the filter might send the message to Azure Service Bus. In case of a consuming pipeline, the filter might use
our [Typed Handling Filter](typed-handling.md) to invoke a service in your business layer.

Meceqs includes filters for typed in-process handling and also for different transports.