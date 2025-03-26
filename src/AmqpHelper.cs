using System.Text;
using Amqp;
using Amqp.Types;

namespace Korjn.AmqpClientInject;

public class AmqpHelper
{
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

            result.ApplicationProperties = new()
            {
                ["_AMQ_DUPL_ID"] = messageId,
            };
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

    public static AmqpMessage CreateMessage(Message message)
    {
        var result = new AmqpMessage
        {
            SenderId = message.Properties?.UserId is null ? default : Encoding.UTF8.GetString(message.Properties.UserId),
            GroupId = message.ApplicationProperties?["_AMQ_GROUP_ID"]?.ToString(),
            Correlation = message.Properties?.CorrelationId ?? throw new NullReferenceException("CorrelationId"),
            Subject = message.Properties?.Subject,
            EnqueueTime = message.Properties?.CreationTime,
            MessageId = message.Properties?.MessageId,
            Attempts = int.Parse(message.ApplicationProperties?["_AMQ-ATTEMPTS"]?.ToString() ?? "0"),
            ContentType = message.Properties?.ContentType,
            Content = message.Body?.ToString() ?? throw new NullReferenceException("Body")
        };

        return result;
    }

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