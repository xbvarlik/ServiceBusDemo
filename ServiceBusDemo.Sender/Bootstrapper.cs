﻿using System.Diagnostics.CodeAnalysis;
using ServiceBusDemo.Sender.Observers;
using ServiceBusDemo.Sender.Services;

namespace ServiceBusDemo.Sender;

public static class Bootstrapper
{
    public static void AddBootstrapper(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationControllersConfiguration();
        services.AddEndpointsApiExplorer();
        services.AddApplicationServices();
        services.AddObservers();
        services.AddSwaggerGen();
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
        services.AddSingleton<QueueService>();
        services.AddScoped<TriggerService>();
    }
    
    private static void AddObservers(this IServiceCollection services)
    {
        services.AddScoped<TriggerSubject>(sp =>
        {
            var triggerSubject = new TriggerSubject();
            
            triggerSubject.RegisterObserver(new DemoQueueTriggerObserver(sp.GetRequiredService<QueueService>()));
            
            return triggerSubject;
        });
    }
    
    private static void AddApplicationControllersConfiguration(this IServiceCollection services)
    {
        services.AddControllers();
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