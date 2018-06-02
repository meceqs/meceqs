# Meceqs

Meceqs is a modular messaging framework that can be used for in-process and out-of-process messaging.

Your __messages__ will be wrapped in __envelopes__ that contain headers and useful diagnostics/tracing data.
The envelopes are sent to __pipelines__ which consist of pluggable __middleware components__.
It's easy to write your own __middleware__ that either enriches/modifies your envelopes or sends them to any target you like
(e.g. a HTTP endpoint, a message broker, a database, ...).

Meceqs targets [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/articles/standard/library) so it can be used
on .NET Core, .NET Framework, Mono, Xamarin and the Universal Windows Platform.

Meceqs ships with the following integrations:

* Strongly typed in-process dispatching
  * Use strongly typed handlers to decouple your domain (Similar to [MediatR](https://github.com/jbogard/MediatR))
* ASP.NET Core
  * Write your own MVC controllers to leverage the features of ASP.NET Core (serializer, routes, validation, ...)
  * Or use our convention-based endpoint for your messages to offer Web APIs without having to write your own MVC controllers.
  * Meceqs attaches itself to the ASP.NET Core request to propagate HttpContext properties like `RequestServices` and `User`.
* HTTP Sender (System.Net.Http.HttpClient)
  * Send messages via HTTP - works best with our convention-based ASP.NET Core API.
* Azure Service Bus
  * Send messages to Azure Service Bus
  * Receive messages from Azure Service Bus
  * Use a file-based mock which makes local development very easy.
* Azure Event Hubs
  * Send messages to Azure Event Hubs
  * Receive messages from Azure Event Hubs
  * Use a a file-based mock which makes local development very easy.
* JSON serialization (enabled by default)

## A first look

The following demo scenario should give you a good first look at Meceqs:

* Your ASP.NET Core frontend has a sign-up page that sends a HTTP request with a `CreateCustomerCommand` to the backend Web API of your customer context.
* Your ASP.NET Core Web API from your customer context receives the command and invokes the handler in your business layer.
* The business layer code decides to forward the message to *Azure Service Bus* because it is too complex to process immediately.
* Your Azure Service Bus host process will receive the message and invoke another handler in your business layer.
* The business layer code will create a new customer, store it in a database and publish an event to *Azure Event Hubs*.

### Frontend web application
Your ASP.NET Core frontend has a sign-up page that sends a HTTP request with a `CreateCustomerCommand` to the backend Web API of your customer context.

#### Usage
```csharp
// Your message can be any arbitrary .NET class - it does not have to implement any interface from Meceqs.
public class CreateCustomerCommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

var cmd = new CreateCustomerCommand { FirstName = "John", LastName = "Snow" };

// IMessageSender is the main interface for sending new messages to a pipeline.
var result = await _messageSender.SendAsync<CreateCustomerResult>(cmd);

Debug.WriteLine("CustomerId: " + result.CustomerId);
```

#### Configuration
Meceqs uses the new abstraction libraries from Microsoft for [dependency injection](https://docs.asp.net/en/latest/fundamentals/dependency-injection.html),
[logging](https://docs.asp.net/en/latest/fundamentals/logging.html) and [configuration](https://docs.asp.net/en/latest/fundamentals/configuration.html).
This allows you to use your favorite DI container, logging framework and configuration source.

This is the configuration for your [ASP.NET Core startup](https://docs.asp.net/en/latest/fundamentals/startup.html) file:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMeceqs()

        // This will serialize envelopes using the JSON format.
        .AddJsonSerialization()

        // You can mix IConfiguration based configuration with static configuration.
        .AddHttpSender(Configuration["HttpSender"], sender =>
        {
            // You can have multiple endpoints to talk to different services.
            sender.AddEndpoint("CustomersApi", options =>
            {
                // You can add regular "DelegatingHandler"s - e.g. to add an Authorization header to each request.
                options.AddDelegatingHandler<OAuthDelegatingHandler>();

                options.AddMessage<CreateCustomerCommand>();
            });
        });
}
```

A json-based configuration file contains settings that change per environment:
```json
{
  "HttpSender": {
    "CustomersApi": {
      "BaseAddress": "http://api.example.com/customers/"
    }
  }
}
```

#### Envelopes
This envelope will be sent to the Web API. The `correlationId` is equal to the `messageId` because this envelope doesn't have a predecessor.
```json
{
  "message": {
    "firstName": "John",
    "lastName": "Snow"
  },
  "messageId": "49f32326-a4a3-4242-9d8f-396c35db2f67",
  "messageType": "Customers.Contracts.Commands.CreateCustomerCommand",
  "correlationId": "49f32326-a4a3-4242-9d8f-396c35db2f67",
  "createdOnUtc": "2016-09-17T19:48:28.5447192Z",
  "headers": {}
}
```

### ASP.NET Core Web API
Your ASP.NET Core Web API from your customer context receives the command and invokes the handler in your business layer.
The business layer code decides to forward the message to *Azure Service Bus* because it is too complex to process immediately.

#### Usage
```csharp
// `IHandles` from "Meceqs.TypedHandling" allows you to handle messages in a strongly typed way.
public class CreateCustomerForwarder : IHandles<CreateCustomerCommand, CreateCustomerResult>
{
    public async Task<CreateCustomerResult> HandleAsync(CreateCustomerCommand msg, HandleContext context)
    {
        // The "HandleContext" gives you access to the envelope and to additional data
        // like the current user, cancellation tokens, ...
        Guid customerId = context.Envelope.MessageId;

        // Since sending new messages from a handler is very common,
        // the context also gives you immediate access to the message sender!
        // This saves you from having to inject it into your constructor.

        // This time we use the builder pattern of IMessageSender to use a named pipeline.
        // This allows you to use multiple pipelines for different use cases.
        await context.MessageSender.ForEnvelope(context.Envelope)
            .UsePipeline(MyPipelines.SendServiceBus)
            .SendAsync();

        return new CreateCustomerResult { CustomerId = customerId };
    }
}
```

#### Configuration
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMeceqs()

        // Configures the behavior of the ASP.NET Core receiver.
        .AddAspNetCoreReceiver(receiver =>
        {
            // The receiver should forward messages to the handler from above.
            receiver.UseTypedHandling(options =>
            {
                options.Handlers.Add<CreateCustomerForwarder>();
            });
        })

        // Add the sender and read the connection string from a configuration source (e.g. Azure Key Vault)
        .AddServiceBusSender(Configuration["ServiceBus"], sender =>
        {
            sender.SetPipelineName(MyPipelines.SendServiceBus);
        });
}

public void Configure(IApplicationBuilder app)
{
    // This adds the receiver to the ASP.NET Core pipeline.
    app.UseMeceqs();
}
```

#### Envelopes
Azure Service Bus will receive this envelope. It has the same MessageId because the message is just forwarded.
```json
{
  "message": {
    "firstName": "John",
    "lastName": "Snow"
  },
  "messageId": "49f32326-a4a3-4242-9d8f-396c35db2f67",
  "messageType": "Customers.Contracts.Commands.CreateCustomerCommand",
  "correlationId": "49f32326-a4a3-4242-9d8f-396c35db2f67",
  "createdOnUtc": "2016-09-17T19:48:28.5447192Z",
  "headers": {}
}
```

### Azure Service Bus Host
Your Azure Service Bus host process will receive the message and invoke another handler in your business layer.
The business layer code will create a new customer, store it in a database and publish an event to *Azure Event Hubs*.

#### Usage
```csharp
// Your Azure Service Bus host will read the BrokeredMessage and call this code to "receive" the envelope
public Task ProcessMessage(BrokeredMessage message)
{
    return _serviceBusReceiver.ReceiveAsync(message, _cancellationToken);
}

// Business layer handler
public class CreateCustomerProcessor : IHandles<CreateCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerProcessor(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task HandleAsync(CreateCustomerCommand cmd, HandleContext context)
    {
        var customerId = context.Envelope.MessageId;

        var customer = new Customer(customerId, cmd.FirstName, cmd.LastName);

        // Saves the customer do a database

        await _customerRepository.Add(customer);

        // Raise an event

        // For demo purposes, we ignore the fact that this creates a new messageId and therefore
        // isn't completely idempotent. Receivers would have to de-duplicate based on the customerId.

        var customerCreatedEvent = new CustomerCreatedEvent(customerId, customer.FirstName, customer.LastName);

        await context.MessageSender.ForMessage(customerCreatedEvent)
            .FollowsFrom(context) // This will correlate the messages
            .UsePipeline(MyPipelines.SendEventHub)
            .SendAsync();
    }
}
```

#### Configuration
```csharp
services.AddMeceqs()
    .AddServiceBusReceiver(receiver =>
    {
        receiver.UseTypedHandling(options =>
        {
            options.Handlers.Add<CreateCustomerProcessor>();
        });
    })
    .AddEventHubSender(Configuration["EventHub"], sender =>
    {
        sender.SetPipelineName(MyPipelines.SendEventHub);
    });
```

#### Envelopes
The event is a new message that again will have the same `correlationId`.
```json
{
  "message": {
    "customerId": "49f32326-a4a3-4242-9d8f-396c35db2f67",
    "firstName": "John",
    "lastName": "Snow"
  },
  "messageId": "e8065827-4321-404f-9071-d8ea5700169d",
  "messageType": "Customers.Contracts.Events.CustomerCreatedEvent",
  "correlationId": "49f32326-a4a3-4242-9d8f-396c35db2f67",
  "createdOnUtc": "2016-09-17T19:48:28.93049202Z",
  "headers": {}
}
```