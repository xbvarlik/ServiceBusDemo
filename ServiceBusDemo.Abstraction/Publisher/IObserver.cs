namespace ServiceBusDemo.Abstraction.Publisher;

public interface IObserver
{
    Task OnEventOccured<T>(T message);
}