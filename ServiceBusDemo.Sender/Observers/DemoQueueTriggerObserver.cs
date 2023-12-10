using ServiceBusDemo.Sender.Constants;
using ServiceBusDemo.Sender.Services;

namespace ServiceBusDemo.Sender.Observers;

public class DemoQueueTriggerObserver(QueueService queueService) : ITriggerObserver
{
    private readonly string _queueName = ServiceBusConstants.DemoQueue;
    
    public async Task OnTriggered<T>(T message)
    {
        await queueService.SendMessageAsync(message, _queueName);
    }
}