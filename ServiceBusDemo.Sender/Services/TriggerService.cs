using ServiceBusDemo.Sender.Models;

namespace ServiceBusDemo.Sender.Services;

public class TriggerService
{
    public async Task<string> CreateTriggerAsync()
    {
        return await Task.FromResult("Message triggered");
    }
    
    public async Task<TriggerModel> CreateTriggerModelAsync(TriggerModel triggerModel)
    {
        return await Task.FromResult(triggerModel);
    }
}