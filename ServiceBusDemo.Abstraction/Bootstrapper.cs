using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceBusDemo.Abstraction.Consumer;
using ServiceBusDemo.Abstraction.Publisher;

namespace ServiceBusDemo.Abstraction;

public static class Bootstrapper
{
    public static void AddServiceBusDemo(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<QueueHandlerRegistry>();
        
        var serviceBusOptions = configuration.GetSection("ServiceBusOptions").Get<ServiceBusOptions>();

        if (serviceBusOptions == null)
            throw new InvalidOperationException("ServiceBusOptions not found in configuration");
        
        services.AddSingleton(serviceBusOptions);

        services.AddScoped(_ => new QueuePublisherService(serviceBusOptions));
        
        services.AddHostedService(serviceProvider => 
            new QueueReceiverHostedService(
                serviceBusOptions, 
                serviceProvider.GetRequiredService<ILogger<QueueReceiverHostedService>>(),
                serviceProvider.GetRequiredService<QueueHandlerRegistry>())
        );
    }

    public static void AddPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusOptions = configuration.GetSection("ServiceBusOptions").Get<ServiceBusOptions>();

        if (serviceBusOptions == null)
            throw new InvalidOperationException("ServiceBusOptions not found in configuration");
        
        services.AddScoped(_ => new QueuePublisherService(serviceBusOptions));
    }
    
    public static IServiceCollection AddSubject<TSubject>(this IServiceCollection services) 
        where TSubject : Subject, new()
    {
        ServiceBusConfiguration.LastRegisteredSubjectType = typeof(TSubject);
        
        services.AddScoped<TSubject>(sp =>
        {
            var subject = ActivatorUtilities.CreateInstance<TSubject>(sp);
            return subject;
        });

        // services.AddScoped<TSubject>(_ => new TSubject());
        
        return services;
    }
    
    public static TSubject CreateSubject<TSubject>(this IServiceProvider serviceProvider) 
        where TSubject : Subject
    {
        var subject = ActivatorUtilities.CreateInstance<TSubject>(serviceProvider);
        return subject;
    }

    public static TSubject CreateObserver<TSubject, TObserver>(this TSubject subject, IServiceProvider serviceProvider)
    where TObserver : IObserver, new()
    where TSubject : Subject
    {
        var observer = ActivatorUtilities.CreateInstance<TObserver>(serviceProvider);
        subject.RegisterObserver(observer);
        return subject;
    }
    
    public static IServiceCollection AddObserver<TObserver>(this IServiceCollection services) 
        where TObserver : Observer
    {
        var subjectType = ServiceBusConfiguration.LastRegisteredSubjectType;

        if (subjectType == null)
            throw new InvalidOperationException("No subject type registered. Please call AddSubject before AddObserver.");

        services.AddScoped(typeof(TObserver), sp =>
        {
            var subject = (Subject)sp.GetRequiredService(subjectType);
            var observer = ActivatorUtilities.CreateInstance<TObserver>(sp);
            subject.RegisterObserver(observer);
            return observer;
        });

        return services;
    }
    
    public static void AddMessageHandler<TMessageHandler>(this IServiceCollection services, string queueName)
        where TMessageHandler : IMessageHandler
    {
        services.AddSingleton(typeof(TMessageHandler), sp =>
        {
            var registry = sp.GetRequiredService<QueueHandlerRegistry>();
            var handler = ActivatorUtilities.CreateInstance<TMessageHandler>(sp);
            registry.RegisterHandler(queueName, handler);
            return handler;
        });
    }

    private static class ServiceBusConfiguration
    {
        public static Type LastRegisteredSubjectType { get; set; } = default!;
    }
}