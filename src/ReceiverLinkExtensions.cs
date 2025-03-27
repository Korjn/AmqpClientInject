using Amqp;

namespace Korjn.AmqpClientInject;

public static class ReceiverLinkExtensions
{
    public static void Commit(this ReceiverLink receiver, Message message)
    {
        receiver?.Accept(message);
    }

    public static void Rollback(this ReceiverLink receiver, Message message)
    {
        receiver?.Modify(message, true);
    }
}
