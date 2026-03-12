using Arcturus.CommandLine;
using Arcturus.CommandLine.Sample.Commands;
using Arcturus.CommandLine.Sample.Middleware;
using Arcturus.CommandLine.Sample.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();
builder.Services.AddTransient<MessageService>();
builder.Services.AddCommandLine();

var host = builder.Build();
host.UseMiddleware<LogMiddleware>();

await host.RunCommandLine<CommandRoot>(["sample", "-rq", "abc"]); // args