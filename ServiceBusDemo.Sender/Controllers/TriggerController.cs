using Microsoft.AspNetCore.Mvc;
using ServiceBusDemo.Sender.Constants;
using ServiceBusDemo.Sender.Models;
using ServiceBusDemo.Sender.Observers;
using ServiceBusDemo.Sender.Services;

namespace ServiceBusDemo.Sender.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TriggerController(TriggerService triggerService, TriggerSubject triggerSubject) : ControllerBase
{
    [HttpGet]
    public async Task<string> CreateTriggerAsync()
    {
        return await triggerService.CreateTriggerAsync();
    }
    
    [HttpPost]
    public async Task<TriggerModel> CreateTriggerModelAsync(TriggerModel triggerModel)
    {
        var model = await triggerService.CreateTriggerModelAsync(triggerModel);

        await triggerSubject.NotifyObserversAsync(model);

        return model;
    }
}