using ServiceBusDemo.Abstraction.Publisher;
using ServiceBusDemo.Sender.Constants;

namespace ServiceBusDemo.Sender.Observers;

public class DemoQueueTriggerObserver : Observer
{
    public DemoQueueTriggerObserver() 
    {
    }
    public DemoQueueTriggerObserver(QueuePublisherService service) : base(service, ServiceBusConstants.DemoQueue)
    {
    }
}