namespace Korjn.AmqpClientInject;

public record AmqpMessage
{
    public string? SenderId { get; init; }
    public string? GroupId { get; init; }
    public string? MessageId { get; init; }
    public string? Subject { get; init; }
    public DateTime? EnqueueTime { get; internal init; }
    public int Attempts { get; internal init; }
    public string Correlation { get; init; } = string.Empty;
    public string? ContentType { get; init; }
    public string Content { get; init; } = string.Empty;
}