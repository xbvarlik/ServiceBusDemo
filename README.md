# Service Bus Sample Application

Service Bus is a message broker that achieves asynchronous communication between multiple applications using message queues and topics.
This project contains only queues, not topics. 

Solution has two project, one sender and one receiver, both web applications in .NET.

## Sender

* Sender has a `QueueService.cs` which encapsulates message publishing logic. 
* `TriggerService.cs` is just an example service to have a function to trigger.
* It implements an Observer Pattern to add flexibility to the project, if one wants to add multiple actions to one trigger,
they can do it without changing the implementation of message publishing logic or event logic. 
* `TriggerController.cs` is a basic controller that just calls `NotifyObservers(model)` method to trigger message publishing.

## Receiver

* Receiver has a hosted service to continuously listen the queues specified in app settings.
* It also has a `QueueHandlerRegistry.cs` to be able to add new queues to service in case the need arises.
* If one wants to add a new queue with a new handler, all to do is create a class that implements `IMessageHandler` and 
register it in the `Bootstrapper.cs` like the previous ones.
* This simplified example has one handler called `LogMessageHandler` which is obviously logs the message into the console. 