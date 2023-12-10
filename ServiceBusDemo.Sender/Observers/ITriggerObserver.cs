namespace ServiceBusDemo.Sender.Observers;

public interface ITriggerObserver
{
    Task OnTriggered<T>(T message);
}