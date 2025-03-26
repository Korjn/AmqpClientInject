namespace Korjn.AmqpClientInject;

public record ConnectionOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }    
    public string? ApplicationName { get; set; }
}