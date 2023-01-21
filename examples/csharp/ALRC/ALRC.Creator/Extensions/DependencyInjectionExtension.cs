using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ALRC.Creator.Extensions;

public static class DependencyInjectionExtension
{
    public static void AddAllImplementationsOf<T>(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton, Assembly? scanningAssembly = null)
    {
        var assembly = scanningAssembly ?? Assembly.GetExecutingAssembly();
        foreach (var type in assembly.GetTypes())
        {
            if (!typeof(T).IsAssignableFrom(type) || type.IsInterface || type.IsAbstract) continue;
            services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
            services.Add(new ServiceDescriptor(type, type, lifetime));
        }
    }
}

public static class DI
{
    public static IServiceProvider Services { get; set; } = null!;
}