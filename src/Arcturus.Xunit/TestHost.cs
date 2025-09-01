using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Arcturus.Xunit;

public abstract class TestHost : IClassFixture<TestHost>
{
    private readonly CancellationTokenSource _cancellationTokenSource;

    public TestHost()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        var hostBuilder = Host.CreateDefaultBuilder();
        var host = hostBuilder
            .ConfigureServices((context, services) =>
            {
                ConfigureServices(services, context.Configuration);
            })
            .Build();

        ServiceProvider = host.Services;
    }
    public abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    public IServiceProvider ServiceProvider { get; }
    public CancellationToken CancellationToken => _cancellationTokenSource.Token;
    public T GetRequiredService<T>(ITestOutputHelper testOutputHelper)
    {
        return ServiceProvider.GetRequiredService<T>();
    }
}
