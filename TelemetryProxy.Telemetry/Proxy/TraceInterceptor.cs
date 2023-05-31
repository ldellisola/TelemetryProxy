using System.Diagnostics;
using System.Text;
using Castle.Core.Internal;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using TelemetryProxy.Telemetry.Metrics;

namespace TelemetryProxy.Telemetry.Proxy;

public class TraceInterceptor<TTracer> : IAsyncInterceptor
where TTracer: class, IServiceTracer
{
    private readonly IServiceProvider _serviceProvider;
    private IServiceTracer _tracer;
    private string _operationName = string.Empty;
    private Dictionary<string,object?>? _tags;

    public TraceInterceptor(TTracer tracer, IServiceProvider serviceProvider)
    {
        _tracer = tracer;
        _serviceProvider = serviceProvider;
    }
    
    public void InterceptSynchronous(IInvocation invocation)
    {
        ParseMethodInformation(invocation);
        var activity = _tracer.Trace(_operationName, _tags);
        try
        {
            invocation.Proceed();
        }
        catch (Exception e)
        {
            activity?.SetStatus(ActivityStatusCode.Error, e.Message);
            throw;
        }
        finally
        {
            activity?.Dispose();
        }
    }

    public void InterceptAsynchronous(IInvocation invocation)
    {
        ParseMethodInformation(invocation);
        invocation.ReturnValue = InterceptAsynchronousInternal(invocation);
    }
    
    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        ParseMethodInformation(invocation);
        invocation.ReturnValue = InterceptAsynchronousInternal<TResult>(invocation);
    }
    
    /// <summary>
    /// It parses the method information and sets the tracer and the operation name
    /// </summary>
    private void ParseMethodInformation(IInvocation invocation)
    {
        var traceAttribute = invocation.Method.GetAttributes<TraceMethodAttribute>().First();
        
        if (traceAttribute.UseFullMethodName)
            _operationName = invocation.Method.DeclaringType?.FullName + "." + invocation.Method.Name;
        else
            _operationName = invocation.Method.Name;

        if (traceAttribute.TracerType is not null)
            _tracer = (_serviceProvider.GetRequiredService(traceAttribute.TracerType) as IServiceTracer)!;

        if(traceAttribute.Enrich)
        {
            _tags = new()
                    {
                        ["Method Call"] = RecreateMethodCall(invocation)
                    };
        }
        else
            _tags = null;

    }

    /// <summary>
    /// It parses the invocation and creates a string with the method call
    /// </summary>
    private static string RecreateMethodCall(IInvocation invocation)
    {
        var builder = new StringBuilder(100);

        builder.Append(invocation.Method.ReturnType.Name);
        builder.Append(' ');
        builder.Append(invocation.Method.Name);
        builder.Append('(');

        var parameters = invocation.Method.GetParameters();
        foreach(var (i, arg) in invocation.Arguments.Select((t, i) => (i, t)))
        {
            var p = parameters[i];
            builder.Append(p.ParameterType.Name);
            if (p.IsOptional)
                builder.Append('?');
            builder.Append(' ');
            builder.Append(p.Name);
            builder.Append(" = ");
            builder.Append(arg switch
            {
                string => $"\"{arg}\"",
                null => "null",
                _ => arg.ToString()
            });
            if (i < invocation.Arguments.Length-1)
                builder.Append(", ");
        }
        builder.Append(')');
        return builder.ToString();
    }

    /// <summary>
    /// It intercepts the asynchronous method call
    /// </summary>
    private async Task InterceptAsynchronousInternal(IInvocation invocation)
    {
        var activity = _tracer.Trace(_operationName, _tags);
        try
        {
            invocation.Proceed();
            await (Task) invocation.ReturnValue;
        }
        catch (Exception e)
        {
            activity?.SetStatus(ActivityStatusCode.Error, e.Message);
            throw;
        }
        finally
        {
            activity?.Dispose();
        }
    }
    
    /// <summary>
    /// It intercepts the asynchronous method call that returns a value
    /// </summary>
    private async Task<TResult>InterceptAsynchronousInternal<TResult>(IInvocation invocation)
    {
        var activity = _tracer.Trace(_operationName, _tags);
        
        try
        {
            invocation.Proceed();
            var result = await (Task<TResult>) invocation.ReturnValue;
            return result;
        }
        catch (Exception e)
        {
            activity?.SetStatus(ActivityStatusCode.Error, e.Message);
            throw;
        }
        finally
        {
            activity?.Dispose();
        }
    }

}