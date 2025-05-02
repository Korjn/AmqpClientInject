namespace Korjn.AmqpClientInject;

/// <summary>
/// Represents connection options for an AMQP client.
/// </summary>
public record AmqpConnectionOptions
{
    /// <summary>
    /// Gets or sets the host address of the AMQP broker.
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// Gets or sets the port number for the AMQP connection.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the username for authentication.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the password for authentication.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the application name for identification.
    /// </summary>
    public string? ApplicationName { get; set; }
}