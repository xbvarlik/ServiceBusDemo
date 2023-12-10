using Azure.Messaging.ServiceBus;

namespace ServiceBusDemo.Receiver.Services.MessageHandlers;

public interface IMessageHandler
{
    Task HandleMessageAsync(ServiceBusReceivedMessage message);
}