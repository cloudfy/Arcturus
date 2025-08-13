using Microsoft.Extensions.Hosting;
using System.CommandLine.Builder;

namespace Arcturus.CommandLine;

public static class HostBuilderExtensions
{
    private static Action<CommandLineBuilder> _configureCommandLineBuilder = (builder) =>
    {
        builder.UseDefaults();
    };

    internal static void AssignCommandLineBuilder(CommandLineBuilder commandLineBuilder)
    {
        _configureCommandLineBuilder(commandLineBuilder);
    }

    public static IHostBuilder ConfigureCommandLineBuilder(
        this IHostBuilder builder
        , Action<CommandLineBuilder> configureAction)
    {
        _configureCommandLineBuilder = configureAction;
        return builder;
    }
}
