using ServiceBusDemo.Abstraction.Utils;

namespace ServiceBusDemo.Receiver.Services;

public class DemoScopedService
{
    public void DoSomething()
    {
        var logger = LocalLogger.CreateLogger<DemoScopedService>();
        logger.LogInformation("Doing something...");
    }
}