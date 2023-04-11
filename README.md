# TelemetryProxy
This is a proof of concept to see if it is viable to automatically instrument out services with OpenTelemetry tracing.

It works by creating proxy objects through `Castle.Core` and `Castle.Core.AsyncInterceptor`.
More information can be found here:
- https://kozmic.net/dynamic-proxy-tutorial/
- https://github.com/castleproject/Core/blob/master/docs/dynamicproxy-async-interception.md
- https://github.com/JSkimming/Castle.Core.AsyncInterceptor
- https://www.youtube.com/watch?v=vBdf3pe1jNU

## How to trace a method
First, you must have a service that implements an interface with the methods you want to trace:
```csharp
// Service.cs
public interface IService {
  [TraceMethod]
  public void DoSomething();
}

public class Service : IService {

  public void DoSomething() {
    // ...
  }
}
```
We also have to create our tracer:
```csharp
public interface IServiceTracer {
  public static abstract string TracerName { get; }
  public Activity? Trace(string operationName, IDictionary<string, object?>? tags);
}

public class ServiceTracer : IServiceTracer {
  public static string TracerName => "DummyTracer";
   public Activity? Trace(string operationName, IEnumerable<KeyValuePair<string, object?>>? tags = null) 
        => _activitySource.StartActivity(name: operationName,tags:tags); 
}
```

Then we must register both the service and OpenTelemetry it using DI:
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
// ...
services.AddTelemetry(
    builder.Configuration,
    configureTraces: t => t.AddSource(ServiceTracer.TracerName)
  );
builder.Services.AddScopedTraceableService<IService,Service,ServiceTracer>();

var app = builder.Build();
// ...
app.Run();
```
