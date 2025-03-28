using System.Text;
using Amqp;
using Amqp.Types;

namespace Korjn.AmqpClientInject;

/// <summary>
/// Provides helper methods for creating AMQP messages.
/// </summary>
public static class AmqpHelper
{
    /// <summary>
    /// Creates a new AMQP message with the specified properties.
    /// </summary>
    /// <param name="correlation">The correlation ID of the message.</param>
    /// <param name="content">The content of the message.</param>
    /// <param name="subject">The subject of the message (optional).</param>
    /// <param name="contentType">The content type of the message (optional).</param>
    /// <param name="messageId">The unique message ID (optional).</param>
    /// <param name="senderId">The sender's ID (optional).</param>
    /// <param name="groupId">The group ID for the message (optional).</param>
    /// <param name="deliveryDelay">The delay before the message is delivered (optional).</param>
    /// <returns>A new instance of <see cref="Message"/>.</returns>
    public static Message CreateMessage(string correlation,
                                        string content,
                                        string? subject = default,
                                        string? contentType = default,
                                        string? messageId = null,
                                        string? senderId = null,
                                        string? groupId = null,
                                        TimeSpan? deliveryDelay = default)
    {
        var result = new Message(content)
        {
            Header = new()
            {
                Durable = true
            },

            Properties = new()
            {
                CorrelationId = correlation,
                CreationTime = DateTime.Now
            }
        };

        if (!string.IsNullOrEmpty(contentType))
        {
            result.Properties.ContentType = new Symbol(contentType);
        }

        if (!string.IsNullOrEmpty(subject))
        {
            result.Properties.Subject = subject;
        }

        if (!string.IsNullOrEmpty(senderId))
        {
            result.Properties.UserId = Encoding.UTF8.GetBytes(senderId);
        }

        if (!string.IsNullOrEmpty(messageId))
        {
            result.Properties.MessageId = messageId;

            result.ApplicationProperties ??= new();
            result.ApplicationProperties["_AMQ_DUPL_ID"] = messageId;
        }

        if (!string.IsNullOrEmpty(groupId))
        {
            result.ApplicationProperties ??= new();
            result.ApplicationProperties["_AMQ_GROUP_ID"] = groupId;
        }

        if ((deliveryDelay?.TotalMilliseconds ?? 0) > 0)
        {
            result.MessageAnnotations = new();
            result.MessageAnnotations[new Symbol("x-opt-delivery-delay")] = deliveryDelay?.TotalMilliseconds;
        }

        return result;
    }

    /// <summary>
    /// Converts an existing AMQP <see cref="Message"/> into an <see cref="AmqpMessage"/>.
    /// </summary>
    /// <param name="message">The AMQP message to convert.</param>
    /// <returns>An <see cref="AmqpMessage"/> instance with extracted properties.</returns>
    /// <exception cref="ArgumentException">Thrown if the correlation ID or content is null or empty.</exception>
    public static AmqpMessage CreateMessage(Message message)
    {
        ArgumentException.ThrowIfNullOrEmpty(message.Properties?.CorrelationId);
        ArgumentException.ThrowIfNullOrEmpty(message.Body?.ToString());

        var result = new AmqpMessage
        {
            SenderId = message.Properties?.UserId is null ? default : Encoding.UTF8.GetString(message.Properties.UserId),
            GroupId = message.ApplicationProperties?["_AMQ_GROUP_ID"]?.ToString(),
            Correlation = message.Properties!.CorrelationId,
            Subject = message.Properties?.Subject,
            EnqueueTime = message.Properties?.CreationTime,
            MessageId = message.Properties?.MessageId,
            Attempts = int.Parse(message.ApplicationProperties?["_AMQ-ATTEMPTS"]?.ToString() ?? "0"),
            ContentType = message.Properties?.ContentType,
            Content = message.Body.ToString()!
        };

        return result;
    }

    /// <summary>
    /// Creates an AMQP message from an existing <see cref="AmqpMessage"/> instance.
    /// </summary>
    /// <param name="message">The <see cref="AmqpMessage"/> instance to convert.</param>
    /// <param name="deliveryDelay">The delay before the message is delivered (optional).</param>
    /// <returns>A new <see cref="Message"/> instance.</returns>
    public static Message CreateMessage(AmqpMessage message,
                                        TimeSpan? deliveryDelay = default)
    {
        return CreateMessage(message.Correlation,
                             message.Content,
                             message.Subject,
                             message.ContentType,
                             message.MessageId,
                             message.SenderId,
                             message.GroupId,
                             deliveryDelay);
    }
}