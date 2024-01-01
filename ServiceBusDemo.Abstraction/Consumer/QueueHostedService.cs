using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azure;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceBusDemo.Abstraction.Models;

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
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        var logger = loggerFactory.CreateLogger<QueueReceiverHostedService>();
        
        if (_queueNames == null || !_queueNames.Any())
            return;
        
        var receiver = new QueueMessageReceiver(_client);

        var tasks = _queueNames.Select(async queueName =>
        {
            var stopwatch = Stopwatch.StartNew();
            
            var handler = handlerRegistry.GetHandler(queueName);
            await receiver.ReceiveMessageBatchFromQueue(queueName, handler.HandleMessageAsync, stoppingToken);
            
            stopwatch.Stop();
            logger.LogInformation($"Processing is done for the queue: {queueName} in a time of {stopwatch.ElapsedMilliseconds}");
            Console.WriteLine($"Processing is done for the queue: {queueName} in a time of {stopwatch.ElapsedMilliseconds}");
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
        var messages = receiver.ReceiveMessagesAsync(cancellationToken).ToBlockingEnumerable(cancellationToken);
        
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        var logger = loggerFactory.CreateLogger<QueueMessageReceiver>();
        
        if (!messages.Any()) 
            return;

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