using Azure.Messaging.ServiceBus;
using ServiceBusDemo.Abstraction.Consumer;

namespace ServiceBusDemo.Receiver.Services.MessageHandlers;

public class LogMessageHandler(ILogger<LogMessageHandler> logger) : IMessageHandler
{
    private readonly ILogger<LogMessageHandler> _logger = logger;
    public Task HandleMessageAsync(ServiceBusReceivedMessage message)
    {
        _logger.LogInformation($"Received message: {message.Body}");
        
        return Task.CompletedTask;
    }
}