using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TelemetryProxy.Telemetry.Metrics;
using TelemetryProxy.Telemetry.Proxy;

namespace TelemetryProxy.Telemetry;

public static class ServiceCollectionExtensions
{
    
    /// <summary>
    /// It registers a scoped service that will be traced
    /// </summary>
    public static IServiceCollection AddScopedTraceableService<TInterface, TImplementation, TMetrics>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TMetrics: class, IServiceTracer
    {
        services.TryAddSingleton<IProxyGenerator,ProxyGenerator>();
        services.AddScoped<TImplementation>();
        services.TryAddTransient<TraceInterceptor<TMetrics>>();
        services.TryAddSingleton<TMetrics>();
        
        services.AddScoped(GenerateProxyObject<TInterface,TImplementation,TMetrics>);
        
        return services;
    }
    
    /// <summary>
    /// It registers a singleton service that will be traced
    /// </summary>
    public static IServiceCollection AddSingletonTraceableService<TInterface, TImplementation, TMetrics>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TMetrics: class, IServiceTracer
    {
        services.TryAddSingleton<IProxyGenerator,ProxyGenerator>();
        services.AddSingleton<TImplementation>();
        services.TryAddTransient<TraceInterceptor<TMetrics>>();

        services.AddSingleton(GenerateProxyObject<TInterface,TImplementation,TMetrics>);
        
        return services;
    }
    
    /// <summary>
    /// It registers a transient service that will be traced
    /// </summary>
    public static IServiceCollection AddTransientTraceableService<TInterface, TImplementation, TMetrics>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TMetrics: class, IServiceTracer
    {
        services.TryAddSingleton<IProxyGenerator,ProxyGenerator>();
        services.AddTransient<TImplementation>();
        services.TryAddTransient<TraceInterceptor<TMetrics>>();
        
        services.AddTransient(GenerateProxyObject<TInterface,TImplementation,TMetrics>);
        
        return services;
    }

    private static TInterface GenerateProxyObject<TInterface,TImplementation,TTracer>(IServiceProvider provider)
        where TInterface : class
        where TImplementation : class, TInterface
        where TTracer: class, IServiceTracer
    {
        var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
        var implementation = provider.GetRequiredService<TImplementation>();
        var interceptor = provider.GetRequiredService<TraceInterceptor<TTracer>>();
        var options = new ProxyGenerationOptions(new TraceProxyGenerationHook());
        return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation, options, interceptor);
    }
}
