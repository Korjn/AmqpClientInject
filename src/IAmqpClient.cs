using Amqp;

namespace Korjn.AmqpClientInject;

public interface IAmqpClient
{    
    Task<ReceiverLink> CreateReceiverAsync(string name, string address);    
    Task<SenderLink> CreateSenderAsync(string name, string address);    
}
