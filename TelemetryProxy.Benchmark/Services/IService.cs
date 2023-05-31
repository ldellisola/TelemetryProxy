using TelemetryProxy.Telemetry.Proxy;

namespace TelemetryProxy.Benchmark.Services;

public interface IService
{
    [TraceMethod]
    public void DoStuff();
    
    [TraceMethod]
    public void DoNothing();

    public void NonTraced();
}