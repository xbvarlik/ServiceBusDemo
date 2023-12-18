namespace ServiceBusDemo.Abstraction.Publisher;

public abstract class Observer(QueuePublisherService service, string queueName) : IObserver
{
    public virtual async Task OnEventOccured<T>(T message)
    {
        await service.SendMessageAsync(message, queueName);
    }
}