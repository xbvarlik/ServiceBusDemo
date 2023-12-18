namespace ServiceBusDemo.Abstraction.Publisher;

public abstract class Observer : IObserver
{
    public abstract Task OnEventOccured<T>(T message);
}