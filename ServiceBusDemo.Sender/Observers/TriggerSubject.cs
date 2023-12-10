namespace ServiceBusDemo.Sender.Observers;

public class TriggerSubject
{
    private IList<ITriggerObserver> Observers { get; } = new List<ITriggerObserver>();
    
    public async Task NotifyObserversAsync<T>(T message)
    {
        foreach (var observer in Observers)
        {
            await observer.OnTriggered(message);
        }
    }
    
    public void RegisterObserver(ITriggerObserver observer)
    {
        Observers.Add(observer);
    }
    
    public void UnregisterObserver(ITriggerObserver observer)
    {
        Observers.Remove(observer);
    }
}