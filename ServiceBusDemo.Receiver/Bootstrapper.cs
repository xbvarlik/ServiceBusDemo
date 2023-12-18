using System.Diagnostics.CodeAnalysis;
using ServiceBusDemo.Abstraction;
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
    
    private static void AddApplicationControllersConfiguration(this IServiceCollection services)
    {
        services.AddControllers();
    }
    
    private static void ConfigureServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceBusDemo(configuration);
        services.AddMessageHandler<LogMessageHandler>(ServiceBusConstants.DemoQueue);
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