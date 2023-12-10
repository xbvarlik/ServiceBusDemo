using System.Text.Json;
using Azure.Messaging.ServiceBus;
using ServiceBusDemo.Sender.Constants;

namespace ServiceBusDemo.Sender.Services;

public class QueueService(IConfiguration configuration)
{
    private readonly ServiceBusClient _client = new(configuration["ServiceBus:ConnectionString"]);

    public async Task SendMessageAsync(string message, string queueName)
    {
        var sender = _client.CreateSender(queueName);
        var serviceBusMessage = new ServiceBusMessage(message);
        await sender.SendMessageAsync(serviceBusMessage);
    }
    
    public async Task SendMessageAsync<T>(T message, string queueName)
    {
        var sender = _client.CreateSender(queueName);
        var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(message));
        await sender.SendMessageAsync(serviceBusMessage);
    }
    
    public async Task SendMessageBatchAsync<T>(IEnumerable<T> messages, string queueName)
    {
        var sender = _client.CreateSender(queueName);
        var serviceBusMessages = messages.Select(message => 
            new ServiceBusMessage(JsonSerializer.Serialize(message)));
        await sender.SendMessagesAsync(serviceBusMessages);
    }
}
