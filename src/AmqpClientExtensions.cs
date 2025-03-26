using Amqp;

namespace Korjn.AmqpClientInject;

public static class AmqpClientExtensions
{
    public static async Task SendMessageAsync(this IAmqpClient client, string senderName, string address, Message message)
    {
        var sender = await client.CreateSenderAsync(senderName, address);
        sender.Send(message);
    }

    public static async Task SendMessageAsync(this IAmqpClient client, string senderName, string address, AmqpMessage message)
    {
        var _message = AmqpHelper.CreateMessage(message);
        await client.SendMessageAsync(senderName, address, _message);
    }

    public static async Task SendMessageAsync(this IAmqpClient client, string senderName, string address, string correlation, string content, TimeSpan? deliveryDelay = default)
    {
        var message = AmqpHelper.CreateMessage(correlation, content, deliveryDelay: deliveryDelay);
        await client.SendMessageAsync(senderName, address, message);
    }
}
