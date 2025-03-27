using Amqp;

namespace Korjn.AmqpClientInject;

/// <summary>
/// Provides extension methods for sending messages using <see cref="IAmqpClient"/>.
/// </summary>
public static class AmqpClientExtensions
{
    /// <summary>
    /// Sends an AMQP message using the specified sender.
    /// </summary>
    /// <param name="client">The AMQP client instance.</param>
    /// <param name="senderName">The name of the sender link.</param>
    /// <param name="address">The address to send the message to.</param>
    /// <param name="message">The AMQP message to send.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SendMessageAsync(this IAmqpClient client, string senderName, string address, Message message)
    {
        var sender = await client.CreateSenderAsync(senderName, address);
        sender.Send(message);
    }

    /// <summary>
    /// Sends an AMQP message using the specified sender, converting from <see cref="AmqpMessage"/>.
    /// </summary>
    /// <param name="client">The AMQP client instance.</param>
    /// <param name="senderName">The name of the sender link.</param>
    /// <param name="address">The address to send the message to.</param>
    /// <param name="message">The <see cref="AmqpMessage"/> to send.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SendMessageAsync(this IAmqpClient client, string senderName, string address, AmqpMessage message)
    {
        var _message = AmqpHelper.CreateMessage(message);
        await client.SendMessageAsync(senderName, address, _message);
    }

    /// <summary>
    /// Sends an AMQP message using the specified sender with correlation and content.
    /// </summary>
    /// <param name="client">The AMQP client instance.</param>
    /// <param name="senderName">The name of the sender link.</param>
    /// <param name="address">The address to send the message to.</param>
    /// <param name="correlation">The correlation ID for the message.</param>
    /// <param name="content">The content of the message.</param>
    /// <param name="deliveryDelay">The optional delay before message delivery.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SendMessageAsync(this IAmqpClient client, string senderName, string address, string correlation, string content, TimeSpan? deliveryDelay = default)
    {
        var message = AmqpHelper.CreateMessage(correlation, content, deliveryDelay: deliveryDelay);
        await client.SendMessageAsync(senderName, address, message);
    }
}
