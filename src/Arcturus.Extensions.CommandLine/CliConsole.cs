using System.Text.RegularExpressions;

namespace Arcturus.CommandLine;

public static class CliConsole
{
    private static readonly Dictionary<string, ConsoleColor> ColorMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["black"] = ConsoleColor.Black,
        ["darkblue"] = ConsoleColor.DarkBlue,
        ["darkgreen"] = ConsoleColor.DarkGreen,
        ["darkcyan"] = ConsoleColor.DarkCyan,
        ["darkred"] = ConsoleColor.DarkRed,
        ["darkmagenta"] = ConsoleColor.DarkMagenta,
        ["darkyellow"] = ConsoleColor.DarkYellow,
        ["gray"] = ConsoleColor.Gray,
        ["darkgray"] = ConsoleColor.DarkGray,
        ["blue"] = ConsoleColor.Blue,
        ["green"] = ConsoleColor.Green,
        ["cyan"] = ConsoleColor.Cyan,
        ["red"] = ConsoleColor.Red,
        ["magenta"] = ConsoleColor.Magenta,
        ["yellow"] = ConsoleColor.Yellow,
        ["white"] = ConsoleColor.White,
    };

    public static void Write(TextWriter writer, string text)
    {
        WriteMarkup(writer, text);
    }
    public static void WriteLine(TextWriter writer, string text)
    {
        WriteMarkup(writer, text + Environment.NewLine);
    }
    public static void Write(string text)
    {
        WriteMarkup(Console.Out, text);
    }
    public static void WriteLine(string text)
    {
        WriteMarkup(Console.Out, text + Environment.NewLine);
    }

    private static void WriteMarkup(TextWriter writer, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        // Check if we can apply colors - console must be available and not redirected
        var canApplyColors = !Console.IsOutputRedirected && !Console.IsErrorRedirected;

        if (!canApplyColors)
        {
            // For redirected output or non-console writers, strip markup tags and write plain text
            var pattern = @"\[(\/?[^\]]+)\]";
            var plainText = Regex.Replace(text, pattern, string.Empty);
            writer.Write(plainText);
            return;
        }

        var originalColor = Console.ForegroundColor;
        var colorStack = new Stack<ConsoleColor>();

        var pattern2 = @"\[(\/?[^\]]+)\]";
        var matches = Regex.Matches(text, pattern2);
        var lastIndex = 0;

        foreach (Match match in matches)
        {
            // Write text before the tag
            if (match.Index > lastIndex)
            {
                writer.Write(text.Substring(lastIndex, match.Index - lastIndex));
            }

            var tag = match.Groups[1].Value;

            if (tag == "/")
            {
                // Reset to previous color or original
                Console.ForegroundColor = colorStack.Count > 0 ? colorStack.Pop() : originalColor;
            }
            else if (ColorMap.TryGetValue(tag, out var color))
            {
                colorStack.Push(Console.ForegroundColor);
                Console.ForegroundColor = color;
            }
            else
            {
                // Not a recognized tag, write it as-is
                writer.Write(match.Value);
            }

            lastIndex = match.Index + match.Length;
        }

        // Write remaining text
        if (lastIndex < text.Length)
        {
            writer.Write(text.Substring(lastIndex));
        }

        // Reset color at the end
        Console.ForegroundColor = originalColor;
    }
}
