using Arcturus.CommandLine.Abstractions;
using Arcturus.CommandLine.Sample.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arcturus.CommandLine.Sample.Commands;

[Command("sample", "A sample command.")]
internal class SampleCommand : ICommand
{
    [Option("--reqstr", "This is required", ["-rq"])]
    internal required string RequiredString { get; init; }

    [Option("--notreq", "This is not required")]
    internal bool? NotRequired { get; init; }
}

internal class SampleCommandHandler(
    MessageService messageService) 
    : ICommandHandler<SampleCommand>
{
    private readonly MessageService _messageService = messageService;
    public async ValueTask Handle(SampleCommand command, CancellationToken cancellationToken)
    {
        var result = await _messageService.GetMessageAsync();
        Console.WriteLine(result);
    }
}