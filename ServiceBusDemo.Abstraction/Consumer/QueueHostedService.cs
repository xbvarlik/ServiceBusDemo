using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azure;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceBusDemo.Abstraction.Models;
using ServiceBusDemo.Abstraction.Utils;

namespace ServiceBusDemo.Abstraction.Consumer;

public class QueueReceiverHostedService(ServiceBusOptions options, QueueHandlerRegistry handlerRegistry) : BackgroundService
{
    private readonly ServiceBusClient _client = new (options.ConnectionString);
    private readonly IList<string>? _queueNames = options.Queues;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ExecuteMessageQueuesParallel(stoppingToken);
        }
    }

    private async Task ExecuteMessageQueues(CancellationToken stoppingToken)
    {
        if (_queueNames == null || !_queueNames.Any())
            return;
        
        var receiver = new QueueMessageReceiver(_client);
        
        foreach (var queueName in _queueNames!)
        {
            var handler = handlerRegistry.GetHandler(queueName);
            await receiver.ReceiveMessagesFromQueueAsync(queueName, handler.HandleMessageAsync, stoppingToken);
        }
    }
    
    private async Task ExecuteMessageQueuesParallel(CancellationToken stoppingToken)
    {
        if (_queueNames == null || !_queueNames.Any())
            return;
        
        var receiver = new QueueMessageReceiver(_client);

        var tasks = _queueNames.Select(async queueName =>
        {
            var handler = handlerRegistry.GetHandler(queueName);
            await receiver.ReceiveMessageBatchFromQueue(queueName, handler.HandleMessageAsync, stoppingToken);
        });

        await Task.WhenAll(tasks);
    }
}

internal class QueueMessageReceiver(ServiceBusClient client)
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
            await receiver.AbandonMessageAsync(message, null, cancellationToken);
            throw new Exception($"Error occurred while processing message: {message.Body}", ex);
        }
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public async Task ReceiveMessageBatchFromQueue(string queueName,
        Func<ServiceBusReceivedMessage, Task> messageHandler,
        CancellationToken cancellationToken = default)
    {
        var receiver = client.CreateReceiver(queueName);
        var messages = await receiver.ReceiveMessagesAsync(1000, cancellationToken: cancellationToken);
        
        if (!messages.Any()) 
            return;
        
        var logger = LocalLogger.CreateLogger<QueueMessageReceiver>();

        foreach (var message in messages)
        {
            try
            {
                await messageHandler(message);
                await receiver.CompleteMessageAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                await receiver.AbandonMessageAsync(message, null, cancellationToken);
                logger.LogError(ex.Message);
            }
        }
    }
}