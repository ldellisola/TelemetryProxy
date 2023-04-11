using System.Reflection;
using Castle.DynamicProxy;

namespace TelemetryProxy.Telemetry.Proxy;

/// <summary>
/// It filters the methods that will be intercepted.
/// In this case, it will intercept only the methods that have the TraceMethodAttribute
/// </summary>
public class TraceProxyGenerationHook : IProxyGenerationHook
{
    public void MethodsInspected()
    {
    }

    public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
    {
    }

    public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
    {
        return methodInfo.GetCustomAttributes<TraceMethodAttribute>().Any();
    }
}