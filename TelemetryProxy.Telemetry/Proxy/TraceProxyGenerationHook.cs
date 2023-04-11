using System.Reflection;
using Castle.DynamicProxy;

namespace TelemetryProxy.Telemetry.Proxy;

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