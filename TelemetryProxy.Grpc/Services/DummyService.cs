using TelemetryProxy.Telemetry.Metrics;
using TelemetryProxy.Telemetry.Proxy;

namespace TelemetryProxy.Grpc.Services;

public class DummyService : IDummyService
{
    public void DoSomething()
    {
         Thread.Sleep(1000);
        // await Task.Delay(Random.Shared.Next(3,10) * 100);
    }

    public async Task DoSomethingElse(string textParam, int numberParam,User user, Place place, object? nullParam = null)
    {
        await Task.Delay(1000);
        // await Task.Delay(Random.Shared.Next(3,10) * 100);
    }
}

public interface IDummyService
{
    [TraceMethod]
    public void DoSomething();
    
    [TraceMethod<SecretServiceTracer>(useFullMethodName:true,enrich:true)]
    public Task DoSomethingElse(string textParam, int numberParam, User user, Place place, object? nullParam = null);
}

public class User
{
    public required string Name { get; set; }
}

public record Place(string Name, string Country);