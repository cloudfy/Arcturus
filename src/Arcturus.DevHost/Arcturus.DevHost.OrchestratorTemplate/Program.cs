// See https://github.com/cloudfy/Arcturus/wiki/DevHost for more information

using Arcturus.DevHost.Hosting;
using Microsoft.Extensions.Logging;

var builder = OrchestratorHostBuilder.Create();
builder.ConfigureLogging(l => l.SetMinimumLevel(LogLevel.Warning));

//builder
//    .AddExecutable("program.exe")
//    .AddProject<SampleApi>(
//        c => c.WithUrls(["http://localhost:5990"]).WithEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development"));

await builder.Build().RunAsync();