﻿using System.ComponentModel.Design;
using Castle.DynamicProxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RystadEnergy.Shared.Telemetry;
using TelemetryProxy.Telemetry.Metrics;
using TelemetryProxy.Telemetry.Proxy;

namespace TelemetryProxy.Telemetry;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetUpOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTelemetry(
                              configuration, 
                              configureTraces: t=> t
                                                   .AddSource(DummyServiceTracer.TracerName)
                                                   .AddSource(SecretServiceTracer.TracerName)
                             );
        
        services.AddSingleton<DummyServiceTracer>();
        services.AddSingleton<SecretServiceTracer>();
        return services;
    }
    
    public static IServiceCollection AddScopedTraceableService<TInterface, TImplementation, TMetrics>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TMetrics: class, IServiceTracer
    {
        services.TryAddSingleton<IProxyGenerator,ProxyGenerator>();
        services.AddScoped<TImplementation>();
        services.TryAddTransient<TraceInterceptor<TMetrics>>();
        

        services.AddScoped(provider =>
                           {
                               var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
                               var implementation = provider.GetRequiredService<TImplementation>();
                               var interceptor = provider.GetRequiredService<TraceInterceptor<TMetrics>>();
                               var options = new ProxyGenerationOptions(new TraceProxyGenerationHook());
                               return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation, options, interceptor);
                           });
        
        return services;
    }
    
    public static IServiceCollection AddSingletonTraceableService<TInterface, TImplementation, TMetrics>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TMetrics: class, IServiceTracer
    {
        services.TryAddSingleton<IProxyGenerator,ProxyGenerator>();
        services.AddSingleton<TImplementation>();
        services.TryAddTransient<TraceInterceptor<TMetrics>>();
        

        services.AddSingleton(provider =>
                           {
                               var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
                               var implementation = provider.GetRequiredService<TImplementation>();
                               var interceptor = provider.GetRequiredService<TraceInterceptor<TMetrics>>();
                               var options = new ProxyGenerationOptions(new TraceProxyGenerationHook());
                               return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation, options, interceptor);
                           });
        
        return services;
    }
    
    public static IServiceCollection AddTransientTraceableService<TInterface, TImplementation, TMetrics>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TMetrics: class, IServiceTracer
    {
        services.TryAddSingleton<IProxyGenerator,ProxyGenerator>();
        services.AddTransient<TImplementation>();
        services.TryAddTransient<TraceInterceptor<TMetrics>>();
        

        services.AddTransient(provider =>
                              {
                                  var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
                                  var implementation = provider.GetRequiredService<TImplementation>();
                                  var interceptor = provider.GetRequiredService<TraceInterceptor<TMetrics>>();
                                  var options = new ProxyGenerationOptions(new TraceProxyGenerationHook());
                                  return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation, options, interceptor);
                              });
        
        return services;
    }
}