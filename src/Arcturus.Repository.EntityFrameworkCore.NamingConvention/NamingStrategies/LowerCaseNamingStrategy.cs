using Arcturus.Repository.EntityFrameworkCore.NamingConvention.Abstracts;
using System.Globalization;

namespace Arcturus.Repository.EntityFrameworkCore.NamingConvention.NamingStrategies;

public sealed class LowerCaseNamingStrategy : INamingStrategy
{
    private readonly CultureInfo _culture;

    internal LowerCaseNamingStrategy(CultureInfo culture)
        => _culture = culture;

    public string ApplyNaming(string name) => name.ToLower(_culture);
}