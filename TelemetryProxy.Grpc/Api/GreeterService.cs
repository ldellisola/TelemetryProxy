using Grpc.Core;
using TelemetryProxy.Grpc.Services;
using TelemetryProxy.Telemetry.Metrics;

namespace TelemetryProxy.Grpc.Api;

public class GreeterService : Greeter.GreeterBase
{
    private readonly IDummyService _dummyService;
    private readonly IServiceTracer _tracer;

    public GreeterService(IDummyService dummyService, DummyServiceTracer tracer)
    {
        _dummyService = dummyService;
        _tracer = tracer;
    }

    public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        _dummyService.DoSomething();

        await _dummyService.DoSomethingElse(
                                            numberParam: 9999,
                                            textParam: "papapa",
                                            user: new User { Name = "John" },
                                            place: new Place("Paris", "France")
                                           );

        return new HelloReply { Message = "Hello " + request.Name };
    }
    
}
