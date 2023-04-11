using TelemetryProxy.Grpc.Api;
using TelemetryProxy.Grpc.Services;
using TelemetryProxy.Telemetry;
using TelemetryProxy.Telemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.SetUpOpenTelemetry(builder.Configuration);
builder.Services.AddScopedTraceableService<IDummyService,DummyService,DummyServiceTracer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
