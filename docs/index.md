# Meceqs

Meceqs is a modular messaging framework that can be used for in-process and out-of-process messaging. 

# Demo

The following demo code should give you a first look at Meceqs and why we think it is useful. This is the demo scenario:

* Your frontend web app has a sign-up page that sends a HTTP request with a `CreateCustomerCommand` to the customers context Web API.
* Your ASP.NET Core Web API from your customers context receives the command and invokes the handler in your business layer.
* The business layer code decides to forward the message to *Azure Service Bus* because it is too complex to process immediately.
* Your Azure Service Bus host process will consume the message and invoke another handler in your business layer.
* The business layer code will create a new customer, store it in a database and publish an event to *Azure Event Hubs*.

## Frontend web application
Your frontend web app has a sign-up page that sends a HTTP request with a `CreateCustomerCommand` to the customers context Web API.

### Usage
```csharp
var cmd = new CreateCustomerCommand { FirstName = "John", LastName = "Snow" };
var result = await _messageSender.SendAsync<CreateCustomerResult>(cmd);

Debug.WriteLine("CustomerId: " + result.CustomerId);
```

### Configuration
```csharp
services.AddMeceqs()
    .AddJsonSerialization()

    // This will read the URL from a configuration source
    .AddHttpSender(Configuration["HttpSender"], options =>
    {
        options.Endpoints["MyEndpoint"].Messages.AddFromAssembly<CreateCustomerCommand>();
    });
```

### Envelopes
This envelope will be sent to the Web API. MessageId and CorrelationId are equal because this envelope hasn't been correlated with another message.
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

## ASP.NET Core Web API
Your ASP.NET Core Web API from your customers context receives the command and invokes the handler in your business layer.
The business layer code decides to forward the message to *Azure Service Bus* because it is too complex to process immediately.

### Usage
```csharp
// The handler in your business layer
public class CreateCustomerForwarder : IHandles<CreateCustomerCommand, CreateCustomerResult>
{
    private readonly IMessageSender _messageSender;

    public CreateCustomerForwarder(IMessageSender messageSender)
    {
        _messageSender = messageSender;
    }

    public async Task<CreateCustomerResult> HandleAsync(HandleContext<CreateCustomerCommand> context)
    {
        Guid customerId = context.Envelope.MessageId;

        await _messageSender.ForEnvelope(context.Envelope)
            .UsePipeline(MyPipelines.SendServiceBus)
            .SendAsync();
        
        return new CreateCustomerResult { CustomerId = customerId };
    }
}
```

### Configuration
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMeceqs()
        .AddJsonSerialization()
        .AddAspNetCoreConsumer(consumer =>
        {
            consumer.UseTypedHandling(options =>
            {
                options.Handlers.Add<CreateCustomerForwarder>();
            });
        })

        // Add the sender and read the connection string from a configuration source (e.g. Azure Key Vault)
        .AddServiceBusSender(MyPipelines.SendServiceBus, Configuration["ServiceBus"]);
}

// This will listen for requests and forward them to your business layer.
public void Configure(IApplicationBuilder app)
{
    app.UseAspNetCoreConsumer();
}
```

### Envelopes
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

## Azure Service Bus Host
Your Azure Service Bus host process will consume the message and invoke another handler in your business layer.
The business layer code will create a new customer, store it in a database and publish an event to *Azure Event Hubs*.

### Usage
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

### Configuration
```csharp
services.AddMeceqs()
    .AddJsonSerialization()
    .AddServiceBusConsumer(Configuration["ServiceBus"], consumer =>
    {
        consumer.UseTypedHandling(options =>
        {
            options.Handlers.Add<CreateCustomerProcessor>();
        });
    })
    .AddEventHubSender(MyPipelines.SendEventHub, Configuration["EventHub"]);
```

### Envelopes
The event is a new message and will not contain the previous history entries. However, it will have the same correlationId.
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