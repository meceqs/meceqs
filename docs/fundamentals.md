# Fundamentals

The following concepts build the foundation of Meceqs:

* __Message:__ 
    Represents a command/query/event that holds your business data.
* __Envelope:__ 
    Every *message* is wrapped in an *envelope* that holds important metadata for correlation, deduplication, diagnostics, serialization.
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
This means, you can define your messages in a library that does not need to reference any Meceqs library.

We recommend that you follow the CQS pattern and separate messages into *commands*, *queries* and *events*:

```csharp
// A command results in a state change
public class CreateCustomerCommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

// An event represents something that happened
public class CustomerCreatedEvent
{
    public Guid CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

// A query returns data and does not change state
public class GetCustomerQuery
{
    public Guid CustomerId { get; set; }
}
```

## Invoking a pipeline

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

Many applications need both scenarios so there must be an easy way to know which pipeline should be invoked.
Also, invoking a pipeline directly would require you to create the `FilterContext` yourself.
To make this easier and obvious, Meceqs includes two interfaces for *sending* and *consuming* messages:

### Meceqs.Sending.IMessageSender
The interface `Meceqs.Sending.IMessageSender` is used for __sending an existing message__ or 
for __forwarding an existing envelope__ to a send-pipeline.

In our `CreateCustomerCommand` example, the frontend web application would send the command with the following code:

```csharp
var cmd = new CreateCustomerCommand { FirstName = "John", LastName = "Snow" };
await _messageSender.SendAsync(cmd);
```

If the target returns a result for the command, the web application would use this code 
(assuming the target returns a `CreateCustomerResult` object with a `CustomerId` property):

```csharp
var cmd = new CreateCustomerCommand { FirstName = "John", LastName = "Snow" };
var result = await _messageSender.SendAsync<CreateCustomerResult>(cmd);

Debug.WriteLine("CustomerId: " + result.CustomerId);
```

### Meceqs.Consuming.IMessageConsumer
The interface `Meceqs.Consuming.IMessageConsumer` is used for __consuming an existing envelope__ with a consume-pipeline.

Following up in our example, the ASP.NET Core backend API would implement a MVC controller with the following code
to process the `CreateCustomerCommand`:

```csharp
[HttpPost]
public Task<CreateCustomerResult> CreateCustomer([FromBody] Envelope<CreateCustomerCommand> envelope)
{
    return _messageConsumer.ConsumeAsync<CreateCustomerResult>(envelope);
}
```

It's important to understand that invoking a pipeline is a simple in-process method call that uses `Task` and `async/await` to minimize blocking. 
Invoking a pipeline does not start a thread and also doesn't use a queuing system. A pipeline is just 
a list of filter objects that are invoked sequentially.

## Writing a filter

Using the same pipeline concept for sending and consuming messages allows you to write and re-use very simple filters.

Filters are very similar to [ASP.NET Core middleware](https://docs.asp.net/en/latest/fundamentals/middleware.html). 
It's a simple class that takes a `FilterDelegate next` constructor parameter representing the next filter and that
has a `public Task Invoke(FilterContext context)` method. You have to call `_next(context)` in this method to invoke
the next filter. This means, you can also use `using` and `try..catch` blocks around this call.

Filters support dependency injection on their constructor and on the `Invoke` method. It's important to understand
that filters are singleton objects, this means constructor parameters are resolved just once from the root service provider
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