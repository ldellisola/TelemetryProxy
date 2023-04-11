# TelemetryProxy
This is a proof of concept to see if it is viable to automatically instrument out services with OpenTelemetry tracing.

It works by creating proxy objects through `Castle.Core` and `Castle.Core.AsyncInterceptor`.
More information can be found here:
- https://kozmic.net/dynamic-proxy-tutorial/
- https://github.com/castleproject/Core/blob/master/docs/dynamicproxy-async-interception.md
- https://github.com/JSkimming/Castle.Core.AsyncInterceptor
