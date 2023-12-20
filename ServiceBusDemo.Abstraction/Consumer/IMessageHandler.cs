using Azure.Messaging.ServiceBus;

namespace ServiceBusDemo.Abstraction.Consumer;

public interface IMessageHandler
{
    string QueueName { get; set; }
    Task HandleMessageAsync(ServiceBusReceivedMessage message);
}

public abstract class MessageHandler : IMessageHandler
{
    public MessageHandler()
    {
        
    }

    public MessageHandler(string queueName)
    {
        QueueName = queueName;
    }
    
    public virtual string QueueName { get; set; }
    public abstract Task HandleMessageAsync(ServiceBusReceivedMessage message);
}