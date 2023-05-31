using System.Diagnostics;

namespace TelemetryProxy.Telemetry.Metrics;

/// <summary>
/// Main service tracer
/// </summary>
public class DummyServiceTracer : IServiceTracer
{

    public static string TracerName => "DummyTracer";
    private readonly ActivitySource _activitySource = new(TracerName);
    public Activity? Trace(string operationName, ActivityKind kind, IEnumerable<KeyValuePair<string, object?>>? tags = null)
    {
        return _activitySource.StartActivity(name: operationName, kind: kind, tags: tags);
    }
}