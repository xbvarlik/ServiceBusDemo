using Azure.Messaging.ServiceBus;

namespace ServiceBusDemo.Abstraction.Consumer;

public interface IMessageHandler
{
    Task HandleMessageAsync(ServiceBusReceivedMessage message);
}