using Azure.Messaging.ServiceBus;
using ServiceBusDemo.Abstraction.Consumer;
using ServiceBusDemo.Abstraction.Utils;
using ServiceBusDemo.Receiver.Models;
using ServiceBusDemo.Receiver.Utils;
using ServiceBusDemo.Sender.Constants;

namespace ServiceBusDemo.Receiver.Services.MessageHandlers;

public class SecondHandler : MessageHandler
{
    private readonly ILogger<LogMessageHandler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    public override string QueueName { get; set; }

    public SecondHandler() : base()
    {
        
    }
    public SecondHandler(IServiceScopeFactory scopeFactory) : base(ServiceBusConstants.SecondQueue)
    {
        _scopeFactory = scopeFactory;
    }

    public override Task HandleMessageAsync(ServiceBusReceivedMessage message)
    {
        var bundle = ServiceBundleUtility.GetService<DemoServiceBundle>(_scopeFactory);
        var demoScopedService = bundle.DemoScopedService;
        demoScopedService.DoSomething();
        
        return Task.CompletedTask;
    }
}