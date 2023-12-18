using ServiceBusDemo.Abstraction.Publisher;
using ServiceBusDemo.Sender.Constants;

namespace ServiceBusDemo.Sender.Observers;

public class DemoQueueTriggerObserver(QueuePublisherService service) : Observer(service, ServiceBusConstants.DemoQueue)
{ }