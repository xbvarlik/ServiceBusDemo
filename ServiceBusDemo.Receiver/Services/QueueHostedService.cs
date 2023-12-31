﻿using Azure.Messaging.ServiceBus;
using ServiceBusDemo.Receiver.Services.MessageHandlers;
using ServiceBusDemo.Sender.Constants;

namespace ServiceBusDemo.Receiver.Services;

public class QueueReceiverHostedService(IConfiguration configuration, ILogger<QueueReceiverHostedService> logger, QueueHandlerRegistry handlerRegistry) : BackgroundService
{
    private readonly ServiceBusClient _client = new (configuration["ServiceBus:ConnectionString"]);
    private readonly IList<string>? _queueNames = configuration.GetSection("ServiceBus:Queues").Get<List<string>>();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ExecuteMessageQueues(stoppingToken);
            
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ExecuteMessageQueues(CancellationToken stoppingToken)
    {
        if (_queueNames == null || !_queueNames.Any())
            await StopAsync(stoppingToken);
        
        var receiver = new QueueMessageReceiver(_client, logger);
        
        foreach (var queueName in _queueNames!)
        {
            var handler = handlerRegistry.GetHandler(queueName);
            await receiver.ReceiveMessagesFromQueueAsync(queueName, handler.HandleMessageAsync, stoppingToken);
        }
    }
}

internal class QueueMessageReceiver(ServiceBusClient client, ILogger<QueueReceiverHostedService> logger)
{
    public async Task ReceiveMessagesFromQueueAsync(string queueName, Func<ServiceBusReceivedMessage, Task> messageHandler, CancellationToken cancellationToken)
    {
        var receiver = client.CreateReceiver(queueName);
        var message = await receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);

        if (message == null) 
            return;
            
        try
        {
            await messageHandler(message);
            await receiver.CompleteMessageAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message");
            await receiver.AbandonMessageAsync(message, null, cancellationToken);
        }
    }
}