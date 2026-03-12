using System;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;

internal class CustomHelpAction : SynchronousCommandLineAction
{
    private readonly HelpAction _defaultHelp;

    public CustomHelpAction(HelpAction action) => _defaultHelp = action;

    public override int Invoke(ParseResult parseResult)
    {
        Console.WriteLine("Before");

        int result = _defaultHelp.Invoke(parseResult);

        Console.WriteLine("Sample usage: --file input.txt");

        return result;

    }
}