using ServiceBusDemo.Receiver.Services.MessageHandlers;

namespace ServiceBusDemo.Receiver.Services;

public class QueueHandlerRegistry
{
    private readonly IDictionary<string, IMessageHandler> _handlers = new Dictionary<string, IMessageHandler>();

    public void RegisterHandler(string queueName, IMessageHandler handler)
    {
        _handlers[queueName] = handler;
    }

    public IMessageHandler GetHandler(string queueName)
    {
        if (_handlers.TryGetValue(queueName, out var handler))
        {
            return handler;
        }

        throw new KeyNotFoundException($"No handler registered for queue: {queueName}");
    }
}
