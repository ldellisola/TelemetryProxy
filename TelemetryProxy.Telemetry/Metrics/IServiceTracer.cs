using System.Diagnostics;

namespace TelemetryProxy.Telemetry.Metrics;

public interface IServiceTracer
{
    public static abstract string TracerName { get; }
    public Activity? Trace(string operationName, ActivityKind kind, IEnumerable<KeyValuePair<string,object?>>? tags = null);

    public Activity? Trace(string operationName) => Trace(operationName, ActivityKind.Internal);
    public Activity? Trace(string operationName, IDictionary<string, object?>? tags) => Trace(operationName, ActivityKind.Internal, tags?.AsEnumerable());
}