namespace Arcturus.CommandLine.Abstractions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class SubCommandAttribute(params Type[] commands) : Attribute
{
    public Type[]? SubCommands => commands;

}