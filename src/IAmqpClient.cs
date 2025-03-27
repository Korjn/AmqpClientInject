using Amqp;

namespace Korjn.AmqpClientInject;

/// <summary>
/// Defines an interface for an AMQP client to create sender and receiver links.
/// </summary>
public interface IAmqpClient
{
    /// <summary>
    /// Creates a receiver link asynchronously.
    /// </summary>
    /// <param name="name">The name of the receiver link.</param>
    /// <param name="address">The address to listen for messages.</param>
    /// <returns>A task representing the asynchronous operation, returning a <see cref="ReceiverLink"/>.</returns>
    Task<ReceiverLink> CreateReceiverAsync(string name, string address);

    /// <summary>
    /// Creates a sender link asynchronously.
    /// </summary>
    /// <param name="name">The name of the sender link.</param>
    /// <param name="address">The address to send messages to.</param>
    /// <returns>A task representing the asynchronous operation, returning a <see cref="SenderLink"/>.</returns>
    Task<SenderLink> CreateSenderAsync(string name, string address);
}
