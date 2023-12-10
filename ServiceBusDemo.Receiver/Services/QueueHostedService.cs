using Azure.Messaging.ServiceBus;
using ServiceBusDemo.Receiver.Services.MessageHandlers;
using ServiceBusDemo.Sender.Constants;

namespace ServiceBusDemo.Receiver.Services;

public class QueueReceiverHostedService(IConfiguration configuration, ILogger<QueueReceiverHostedService> logger, QueueHandlerRegistry handlerRegistry) : BackgroundService
{
    private readonly ServiceBusClient _client = new (configuration["ServiceBus:ConnectionString"]);
    private readonly IList<string>? _queueNames = configuration.GetSection("ServiceBus:Queues").Get<List<string>>();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}

internal class QueueMessageReceiver(ServiceBusClient client, ILogger<QueueReceiverHostedService> logger)
{
    public async Task ReceiveMessagesFromQueueAsync(string queueName, Func<ServiceBusReceivedMessage, Task> messageHandler, CancellationToken cancellationToken)
    {
        var receiver = client.CreateReceiver(queueName);
        while (!cancellationToken.IsCancellationRequested)
        {
            var message = await receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);

            if (message == null) 
                continue;
            
            try
            {
                // Process the message
                await messageHandler(message);

                // Complete the message upon successful processing
                await receiver.CompleteMessageAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing message");
                await receiver.AbandonMessageAsync(message, null, cancellationToken);
            }
        }
    }
}