using Grpc.Core;
using TelemetryProxy.Grpc.Services;

namespace TelemetryProxy.Grpc.Api;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    private readonly IDummyService _dummyService;
    public GreeterService(ILogger<GreeterService> logger, IDummyService dummyService)
    {
        _logger = logger;
        _dummyService = dummyService;
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
