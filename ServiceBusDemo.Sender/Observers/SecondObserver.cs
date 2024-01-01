using ServiceBusDemo.Abstraction.Publisher;
using ServiceBusDemo.Sender.Constants;

namespace ServiceBusDemo.Sender.Observers;

public class SecondObserver : Observer
{
    public SecondObserver() : base()
    {
        
    }

    public SecondObserver(QueuePublisherService service) : base(service, ServiceBusConstants.SecondQueue)
    {
        
    }
}