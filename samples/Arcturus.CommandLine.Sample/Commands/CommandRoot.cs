using Arcturus.CommandLine.Abstractions;

namespace Arcturus.CommandLine.Sample.Commands;

[Command("root", "The root command")]
[SubCommand(typeof(SampleCommand))]
internal class CommandRoot : CommandLineRoot
{
    
}