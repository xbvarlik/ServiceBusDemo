using Azure.Messaging.ServiceBus;

namespace ServiceBusDemo.Receiver.Services.MessageHandlers;

public class LogMessageHandler(ILogger logger) : IMessageHandler
{
    public Task HandleMessageAsync(ServiceBusReceivedMessage message)
    {
        logger.LogInformation($"Received message: {message.Body}");
        
        return Task.CompletedTask;
    }
}