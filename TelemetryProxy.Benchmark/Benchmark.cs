using BenchmarkDotNet.Attributes;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using TelemetryProxy.Benchmark.Services;
using TelemetryProxy.Telemetry.Metrics;
using TelemetryProxy.Telemetry.Proxy;

namespace TelemetryProxy.Benchmark;

[MemoryDiagnoser()]
public class Benchmark
{
    private IService _nativeTracer = new DummyService();
    private IService _proxyTracer = GetProxyTracer();

    private static IService GetProxyTracer()
    {
        var proxyGenerator = new ProxyGenerator();
        var impl = new DummyService();
        var traceInterceptor = new TraceInterceptor<DummyServiceTracer>(new DummyServiceTracer(),null);
        var options = new ProxyGenerationOptions(new TraceProxyGenerationHook());
        return proxyGenerator.CreateInterfaceProxyWithTarget<IService>(impl, options, traceInterceptor);
    }

    [ Benchmark ]
    public void DoStuff_NativeCall()
    {
        _nativeTracer.DoStuff();
    }


    [ Benchmark ]
    public void DoStuff_ProxiedCall()
    {
        _proxyTracer.DoStuff();
    }
    
    [ Benchmark ]
    public void DoNothing_NativeCall()
    {
        _nativeTracer.DoNothing();
    }


    [ Benchmark ]
    public void DoNothing_ProxiedCall()
    {
        _proxyTracer.DoNothing();
    }
    
    [ Benchmark ]
    public void NonTraced_NativeCall()
    {
        _nativeTracer.NonTraced();
    }


    [ Benchmark ]
    public void NonTraced_ProxiedCall()
    {
        _proxyTracer.NonTraced();
    }
}