using Arcturus.CommandLine.Abstractions;
using System;
using System.Threading.Tasks;

namespace Arcturus.CommandLine.Sample.Middleware;

internal class LogMiddleware : ICommandLineMiddleware
{
    public async Task InvokeAsync(CommandLineContext context, CommandLineDelegate next)
    {
        Console.WriteLine("LogMiddleware: Before next");

        await next(context);

        Console.WriteLine("LogMiddleware: After next");
    }
}
