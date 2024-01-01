using Microsoft.Extensions.DependencyInjection;

namespace ServiceBusDemo.Abstraction.Utils;

public static class ServiceBundleUtility
{
    public static T GetService<T>(IServiceScopeFactory scopeFactory)
        where T : class
    {
        using var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }
}