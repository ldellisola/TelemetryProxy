namespace TelemetryProxy.Benchmark.Services;

public class DummyService : IService
{
    public void DoStuff()
    {
        int i = 0;

        for(int j = 0; j < 1000; j++)
        {
            if(i == j)
                i--;
            else
                i++;
        }
    }

    public void DoNothing()
    {
        
    }

    public void NonTraced()
    {
        
    }
}