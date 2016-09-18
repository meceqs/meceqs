# Meceqs

Meceqs is a modular messaging framework that can be used for in-process and out-of-process messaging.

Your __messages__ will be wrapped in __envelopes__ that contain headers and useful diagnostics/tracing data.
The envelopes are sent to __pipelines__ which consist of pluggable __filters__.
It's easy to write your own __filters__ that either enrich/modify your envelopes or send them to any external system you like
(e.g. a HTTP endpoint, a message broker, a database, ...).

Meceqs targets [.NET Standard 1.3](https://docs.microsoft.com/en-us/dotnet/articles/standard/library) so it can be used
on .NET Core, .NET Framework, Mono, Xamarin and the Universal Windows Platform.

Meceqs ships with the following integrations:

* Strongly typed in-process dispatching
  * Use strongly typed handlers to decouple your domain (Similar to [MediatR](https://github.com/jbogard/MediatR))
* ASP.NET Core
  * Use a convention-based endpoint for your messages to offer Web APIs without having to write your own MVC controllers.
  * Meceqs attaches itself to the ASP.NET Core request and adds useful metadata to every message that is sent or received. (e.g. RequestPath, ...)
* HTTP Sender (System.Net.Http.HttpClient)
  * Send messages via HTTP - works best with the convention-based ASP.NET Core API.
* Azure Service Bus
  * Send messages to Azure Service Bus
  * Consume messages from Azure Service Bus
  * Meceqs also contains a file-based mock which makes local development very easy.
  * *NOTE: There's no official .NET standard compatible Azure Service Bus library yet so this integration only works on the full .NET framework*
* Azure Event Hubs
  * Send messages to Azure Event Hubs
  * Consume messages from Azure Event Hubs
  * Meceqs also contains a file-based mock which makes local development very easy.
  * *NOTE: There's no official .NET standard compatible Azure Event Hubs library yet so this integration only works on the full .NET framework*
* JSON serialization

## A first look

The following demo scenario should give you a good first look at Meceqs:

* Your ASP.NET Core frontend has a sign-up page that sends a HTTP request with a `CreateCustomerCommand` to the backend Web API of your customer context.
* Your ASP.NET Core Web API from your customer context receives the command and invokes the handler in your business layer.
* The business layer code decides to forward the message to *Azure Service Bus* because it is too complex to process immediately.
* Your Azure Service Bus host process will consume the message and invoke another handler in your business layer.
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

// IMessageSender is the main interface for sending messages.
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
            // You could have multiple endpoints to talk to different services.
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
  "messageName": "CreateCustomerCommand",
  "correlationId": "49f32326-a4a3-4242-9d8f-396c35db2f67",
  "createdOnUtc": "2016-09-17T19:48:28.5447192Z",
  "headers": {},
  "history": [
    {
      "pipeline": "Send",
      "host": "FRONTEND01",
      "endpoint": "FrontendWebApp",
      "createdOnUtc": "2016-09-17T19:48:28.5729711Z",
      "properties": {
        "requestId": "0HKUV6E665U19",
        "requestPath": "/SignUp"
      }
    }
  ]
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
    private readonly IMessageSender _messageSender;

    public CreateCustomerForwarder(IMessageSender messageSender)
    {
        _messageSender = messageSender;
    }

    public async Task<CreateCustomerResult> HandleAsync(HandleContext<CreateCustomerCommand> context)
    {
        // The "HandleContext" gives you access to the envelope and to additional data
        // like the current user, cancellation tokens, ...
        Guid customerId = context.Envelope.MessageId;

        // This time we use the builder pattern of IMessageSender to use a named pipeline.
        // This allows you to use multiple pipelines for different use cases.
        await _messageSender.ForEnvelope(context.Envelope)
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
        .AddJsonSerialization()

        // Configures the behavior of the ASP.NET Core consumer.
        .AddAspNetCoreConsumer(consumer =>
        {
            // The consumer should forward messages to the handler from above.
            consumer.UseTypedHandling(options =>
            {
                options.Handlers.Add<CreateCustomerForwarder>();
            });
        })

        // Add the sender and read the connection string from a configuration source (e.g. Azure Key Vault)
        .AddServiceBusSender(MyPipelines.SendServiceBus, Configuration["ServiceBus"]);
}

public void Configure(IApplicationBuilder app)
{
    // This adds the consumer to the ASP.NET Core pipeline.
    app.UseAspNetCoreConsumer();
}
```

#### Envelopes
Azure Service Bus will receive this envelope. It has the same MessageId because the message is just forwarded.
There's one history entry for every pipeline that processed the message.
```json
{
  "message": {
    "firstName": "John",
    "lastName": "Snow"
  },
  "messageId": "49f32326-a4a3-4242-9d8f-396c35db2f67",
  "messageType": "Customers.Contracts.Commands.CreateCustomerCommand",
  "messageName": "CreateCustomerCommand",
  "correlationId": "49f32326-a4a3-4242-9d8f-396c35db2f67",
  "createdOnUtc": "2016-09-17T19:48:28.5447192Z",
  "headers": {},
  "history": [
    {
      "pipeline": "Send",
      "host": "FRONTEND01",
      "endpoint": "FrontendWebApp",
      "createdOnUtc": "2016-09-17T19:48:28.5729711Z",
      "properties": {
        "requestId": "0HKUV6E665U19",
        "requestPath": "/SignUp"
      }
    },
    {
      "pipeline": "Consume",
      "host": "BACKEND01",
      "endpoint": "Customers.Hosts.WebApi",
      "createdOnUtc": "2016-09-17T19:48:28.6304382Z",
      "properties": {
        "requestId": "0HKUV6JTPD1B1",
        "requestPath": "/CreateCustomer"
      }
    },
    {
      "pipeline": "SendServiceBus",
      "host": "BACKEND01",
      "endpoint": "Customers.Hosts.WebApi",
      "createdOnUtc": "2016-09-17T19:48:28.6321242Z",
      "properties": {
        "requestId": "0HKUV6JTPD1B1",
        "requestPath": "/CreateCustomer"
      }
    }
  ]
}
```

### Azure Service Bus Host
Your Azure Service Bus host process will consume the message and invoke another handler in your business layer.
The business layer code will create a new customer, store it in a database and publish an event to *Azure Event Hubs*.

#### Usage
```csharp
// Your Azure Service Bus host will read the BrokeredMessage and call this code to "consume" the envelope
public Task ProcessMessage(BrokeredMessage message)
{
    return _serviceBusConsumer.ConsumeAsync(message, _cancellationToken);
}

// Business layer handler
public class CreateCustomerProcessor : IHandles<CreateCustomerCommand>
{
    private readonly IMessageSender _messageSender;
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerProcessor(IMessageSender messageSender, ICustomerRepository customerRepository)
    {
        _messageSender = messageSender;
        _customerRepository = customerRepository;
    }

    public async Task HandleAsync(HandleContext<CreateCustomerCommand> context)
    {
        var customerId = context.Envelope.MessageId;
        var cmd = context.Message;

        var customer = new Customer(customerId, cmd.FirstName, cmd.LastName);

        // Saves the customer do a database

        await _customerRepository.Add(customer);

        // Raise an event

        // For demo purposes, we ignore the fact that this creates a new messageId and therefore
        // isn't completely idempotent. Consumers would have to de-duplicate based on the customerId.

        var customerCreatedEvent = new CustomerCreatedEvent(customerId, customer.FirstName, customer.LastName);

        await _messageSender.ForMessage(customerCreatedEvent)
            .FollowsFrom(context) // This will correlate the messages
            .UsePipeline(MyPipelines.SendEventHub)
            .SendAsync();
    }
}
```

#### Configuration
```csharp
services.AddMeceqs()
    .AddJsonSerialization()

    .AddServiceBusConsumer(consumer =>
    {
        consumer.UseTypedHandling(options =>
        {
            options.Handlers.Add<CreateCustomerProcessor>();
        });
    })

    .AddEventHubSender(MyPipelines.SendEventHub, Configuration["EventHub"]);
```

#### Envelopes
The event is a new message and will not contain the previous history entries. However, it will have the same `correlationId`.
```json
{
  "message": {
    "customerId": "49f32326-a4a3-4242-9d8f-396c35db2f67",
    "firstName": "John",
    "lastName": "Snow"
  },
  "messageId": "e8065827-4321-404f-9071-d8ea5700169d",
  "messageType": "Customers.Contracts.Events.CustomerCreatedEvent",
  "messageName": "CustomerCreatedEvent",
  "correlationId": "49f32326-a4a3-4242-9d8f-396c35db2f67",
  "createdOnUtc": "2016-09-17T19:48:28.93049202Z",
  "headers": {},
  "history": [
    {
      "pipeline": "SendEventHub",
      "host": "BACKEND02",
      "endpoint": "Customers.Hosts.Worker",
      "createdOnUtc": "2016-09-17T19:48:28.9305021Z",
      "properties": {}
    }
  ]
}
```