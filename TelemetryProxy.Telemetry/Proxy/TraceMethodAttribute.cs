using TelemetryProxy.Telemetry.Metrics;

namespace TelemetryProxy.Telemetry.Proxy;


#if NET7_0_OR_GREATER

/// <summary>
/// It registers a method that will be traced with a custom tracer
/// </summary>
/// <typeparam name="TTracer">Custom tracer</typeparam>
[ AttributeUsage(AttributeTargets.Method) ]
public class TraceMethodAttribute<TTracer> : TraceMethodAttribute
    where TTracer : class, IServiceTracer
{
    public TraceMethodAttribute(bool useFullMethodName = false, bool enrich = false)
    : base(typeof(TTracer),useFullMethodName,enrich)
    {
    }
}
#endif

/// <summary>
/// It registers a method that will be traced
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TraceMethodAttribute : Attribute
{
    /// <summary>
    /// When true, it will use the full method name (including the namespace)
    /// </summary>
    public bool UseFullMethodName { get; }
    
    /// <summary>
    /// When true, it will enrich the trace with the method call
    /// </summary>
    public bool Enrich { get; }
    
    /// <summary>
    /// When not null, it will use the custom tracer
    /// </summary>
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