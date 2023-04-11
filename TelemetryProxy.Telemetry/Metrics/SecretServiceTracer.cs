using System.Diagnostics;

namespace TelemetryProxy.Telemetry.Metrics;

/// <summary>
/// Service tracer used to show how to use a different tracer in a method
/// </summary>
public class SecretServiceTracer : IServiceTracer
{
    public static string TracerName => "SecretTracer";
    private readonly ActivitySource _activitySource = new(TracerName);
    public Activity? Trace(string operationName, ActivityKind kind, IEnumerable<KeyValuePair<string, object?>>? tags = null) 
        => _activitySource.StartActivity(name: operationName, kind: kind,tags:tags);
}