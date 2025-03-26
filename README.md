# AmqpClientInject

AmqpClientInject is a lightweight dependency injection wrapper for integrating AMQP-based messaging (such as RabbitMQ, ActiveMQ Artemis) into .NET applications. It simplifies the configuration and usage of AMQP clients using Microsoft.Extensions.DependencyInjection.

## Features
- Seamless integration with .NET Dependency Injection (DI)
- Simplifies AMQP connection management
- Supports configuring multiple AMQP clients
- Compatible with AMQP 1.0 implementations

## Installation
The package is available on [NuGet](https://www.nuget.org/) (Coming soon). Install it using the following command:

```sh
 dotnet add package AmqpClientInject
```

## Getting Started

### 1. Registering AmqpClientInject in DI
Add the necessary dependencies to your `Program.cs` or `Startup.cs`:

#### Registering a Default AMQP Client
```csharp
using Microsoft.Extensions.DependencyInjection;
using AmqpClientInject;

var services = new ServiceCollection();

services.AddAmqpClient(options =>
{
    options.Host = "localhost";
    options.Port = 5672;
    options.UserName = "guest";
    options.Password = "guest";
});
```

#### Registering Multiple Named AMQP Clients
```csharp
services.AddAmqpClient("Primary", options =>
{
    options.Host = "primary-host";
    options.Port = 5672;
    options.UserName = "user1";
    options.Password = "password1";
});

services.AddAmqpClient("Secondary", options =>
{
    options.Host = "secondary-host";
    options.Port = 5672;
    options.UserName = "user2";
    options.Password = "password2";
});
```

### 2. Using IAmqpClient

#### Creating Receiver and Sender
```csharp
public class AmqpService
{
    private readonly IAmqpClient _amqpClient;

    public AmqpService(IAmqpClient amqpClient)
    {
        _amqpClient = amqpClient;
    }

    public async Task ReceiveMessagesAsync(string receiverName, string address)
    {
        var receiver = await _amqpClient.CreateReceiverAsync(receiverName, address);
        receiver.Start(async (receiver, message) =>
        {
            Console.WriteLine($"Received message: {message.Body}");
            await receiver.AcceptAsync(message);
        });
    }

    public async Task SendMessageAsync(string senderName, string address, string content)
    {
        var sender = await _amqpClient.CreateSenderAsync(senderName, address);
        var message = new Message(content);
        sender.Send(message);
    }
}
```

### 3. Using AmqpClientExtensions for Message Sending
```csharp
public class AmqpMessageService
{
    private readonly IAmqpClient _amqpClient;

    public AmqpMessageService(IAmqpClient amqpClient)
    {
        _amqpClient = amqpClient;
    }

    public async Task SendCustomMessageAsync(string senderName, string address, string correlationId, string content, TimeSpan? delay = null)
    {
        await _amqpClient.SendMessageAsync(senderName, address, correlationId, content, delay);
    }
}
```

## Configuration Options
You can customize connection settings using the `ConnectionOptions` class:

| Option           | Description                          | Default Value |
|-----------------|----------------------------------|--------------|
| Host           | AMQP broker hostname             | `localhost`  |
| Port           | AMQP port                         | `5672`       |
| UserName       | AMQP username                    | `guest`      |
| Password       | AMQP password                    | `guest`      |
| ApplicationName| Optional application name        | `null`       |

Example:
```csharp
services.AddAmqpClient(options =>
{
    options.Host = "amqp.example.com";
    options.Port = 5672;
    options.UserName = "user";
    options.Password = "password";
    options.ApplicationName = "MyApp";
});
```

## Why Use AmqpClientInject?
- Reduces boilerplate AMQP setup code
- Ensures best practices for connection management
- Easily configurable and extendable

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contributing
Contributions are welcome! Feel free to open an issue or submit a pull request.

## Contact
For support or inquiries, please open an issue on GitHub.

