using TelemetryProxy.Telemetry.Metrics;

namespace TelemetryProxy.Telemetry.Proxy;

[ AttributeUsage(AttributeTargets.Method) ]
public class TraceMethodAttribute<TTracer> : TraceMethodAttribute
    where TTracer : class, IServiceTracer
{
    public TraceMethodAttribute(bool useFullMethodName = false, bool enrich = false)
    : base(typeof(TTracer),useFullMethodName,enrich)
    {
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class TraceMethodAttribute : Attribute
{
    public bool UseFullMethodName { get; }
    public bool Enrich { get; }
    public Type? TracerType { get; }

    public TraceMethodAttribute(bool useFullMethodName = false, bool enrich = false)
    {
        UseFullMethodName = useFullMethodName;
        Enrich = enrich;
    }
    
    public TraceMethodAttribute(Type tracerType, bool useFullMethodName = false, bool enrich = false)
    : this(useFullMethodName, enrich)
    {
        TracerType = tracerType;
    }

}