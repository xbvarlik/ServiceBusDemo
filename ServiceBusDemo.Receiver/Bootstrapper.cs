using System.Diagnostics.CodeAnalysis;
using ServiceBusDemo.Receiver.Services;
using ServiceBusDemo.Receiver.Services.MessageHandlers;
using ServiceBusDemo.Sender.Constants;

namespace ServiceBusDemo.Receiver;

public static class Bootstrapper
{
    public static void AddBootstrapper(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationControllersConfiguration();
        services.AddEndpointsApiExplorer();
        services.AddApplicationServices();
        services.AddSwaggerGen();
        services.AddMessageConsumer();
    }

    [SuppressMessage("ReSharper", "InvertIf")]
    public static void AddAppBootstrapper(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.ConfigureApplicationSwagger();
        // app.ConfigureApplicationExceptionHandling();
        app.MapControllers();
        app.UseSwagger(); 
        app.UseSwaggerUI();
        
    }

    private static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddHostedService<QueueReceiverHostedService>();
    }
    
    private static void AddApplicationControllersConfiguration(this IServiceCollection services)
    {
        services.AddControllers();
    }
    
    private static void AddMessageConsumer(this IServiceCollection services)
    {
        services.AddSingleton<LogMessageHandler>();
        
        services.AddSingleton<QueueHandlerRegistry>(sp =>
        {
            var registry = new QueueHandlerRegistry();
            
            registry.RegisterHandler(ServiceBusConstants.DemoQueue, sp.GetRequiredService<LogMessageHandler>());
            
            return registry;
        });
    }
    
    private static void ConfigureApplicationSwagger(this WebApplication app)
    {
        // if (!app.Environment.IsDevelopment()) return;

        app.UseSwagger();
    }
    
    // private static void ConfigureApplicationExceptionHandling(this WebApplication app)
    // {
    //     app.UseExceptionHandler("/error");
    // }
}