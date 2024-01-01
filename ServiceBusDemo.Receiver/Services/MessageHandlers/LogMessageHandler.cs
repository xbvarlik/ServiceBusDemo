﻿using Azure.Messaging.ServiceBus;
using ServiceBusDemo.Abstraction.Consumer;
using ServiceBusDemo.Receiver.Models;
using ServiceBusDemo.Sender.Constants;

namespace ServiceBusDemo.Receiver.Services.MessageHandlers;

public class LogMessageHandler : MessageHandler
{
    private readonly ILogger<LogMessageHandler> _logger;
    public override string QueueName { get; set; }

    public LogMessageHandler() : base()
    {
        
    }
    public LogMessageHandler(ILogger<LogMessageHandler> logger) : base(ServiceBusConstants.DemoQueue)
    {
        _logger = logger;
    }

    public override Task HandleMessageAsync(ServiceBusReceivedMessage message)
    {
        _logger.LogInformation($"Received message: {message.Body}");
        
        var deserializedMessage = DeserializeMessage<TriggerModel>(message);
        
        _logger.LogInformation($"Deserialized message: {deserializedMessage.Message}");
        
        return Task.CompletedTask;
    }
}