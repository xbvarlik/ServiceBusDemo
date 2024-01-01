using System.Text.Json;
using Azure.Messaging.ServiceBus;
using ServiceBusDemo.Abstraction.Models;

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
    
    public virtual async Task HandleMessageBatchAsync(IEnumerable<ServiceBusReceivedMessage> messages)
    {
        foreach (var message in messages)
        {
            await HandleMessageAsync(message);
        }
    }
    
    protected virtual T? DeserializeMessage<T>(ServiceBusReceivedMessage message)
    {
        var messageBody = message.Body.ToString();
        return JsonSerializer.Deserialize<T>(messageBody);
    }
}