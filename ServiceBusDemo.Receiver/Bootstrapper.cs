using System.Diagnostics.CodeAnalysis;
using ServiceBusDemo.Abstraction;
using ServiceBusDemo.Abstraction.Consumer;
using ServiceBusDemo.Receiver.Services.MessageHandlers;
using ServiceBusDemo.Sender.Constants;

namespace ServiceBusDemo.Receiver;

public static class Bootstrapper
{
    public static void AddBootstrapper(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationControllersConfiguration();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.ConfigureServiceBus(configuration);
    }

    public static void AddAppBootstrapper(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.ConfigureApplicationSwagger();
        app.MapControllers();
    }
    
    private static void AddApplicationControllersConfiguration(this IServiceCollection services)
    {
        services.AddControllers();
    }
    
    private static void ConfigureServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<LogMessageHandler>();
        services.AddSingleton<SecondHandler>();
        
        services.AddSingleton<QueueHandlerRegistry>(sp => 
            sp.AddHandlerRegistry()
                .AddMessageHandler<LogMessageHandler>(sp)
                .AddMessageHandler<SecondHandler>(sp)
            );
        
        services.AddSubscriber(configuration);
    }
    
    private static void ConfigureApplicationSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}