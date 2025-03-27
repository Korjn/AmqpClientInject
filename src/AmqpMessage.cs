namespace Korjn.AmqpClientInject;

/// <summary>
/// Represents an AMQP message with relevant properties.
/// </summary>
public record AmqpMessage
{
    /// <summary>
    /// Gets the sender's identifier.
    /// </summary>
    public string? SenderId { get; init; }

    /// <summary>
    /// Gets the group identifier associated with the message.
    /// </summary>
    public string? GroupId { get; init; }

    /// <summary>
    /// Gets the unique identifier of the message.
    /// </summary>
    public string? MessageId { get; init; }

    /// <summary>
    /// Gets the subject of the message.
    /// </summary>
    public string? Subject { get; init; }

    /// <summary>
    /// Gets the timestamp when the message was enqueued.
    /// </summary>
    public DateTime? EnqueueTime { get; internal init; }

    /// <summary>
    /// Gets the number of delivery attempts for the message.
    /// </summary>
    public int Attempts { get; internal init; }

    /// <summary>
    /// Gets the correlation identifier for tracking message processing.
    /// </summary>
    public string Correlation { get; init; } = string.Empty;

    /// <summary>
    /// Gets the content type of the message.
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// Gets the content of the message.
    /// </summary>
    public string Content { get; init; } = string.Empty;
}