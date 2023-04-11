using System.Diagnostics;

namespace TelemetryProxy.Telemetry.Metrics;

public class DummyServiceTracer : IServiceTracer
{
    public static string TracerName => "DummyTracer";
    private readonly ActivitySource _activitySource = new(TracerName);
    public Activity? Trace(string operationName, ActivityKind kind, IEnumerable<KeyValuePair<string, object?>>? tags = null) 
        => _activitySource.StartActivity(name: operationName, kind: kind,tags:tags);
}