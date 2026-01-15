using Arcturus.Repository.EntityFrameworkCore.NamingConvention.Abstracts;
using System.Globalization;

namespace Arcturus.Repository.EntityFrameworkCore.NamingConvention.NamingStrategies;

public sealed class UpperCaseNamingStrategy : INamingStrategy
{
    private readonly CultureInfo _culture;

    internal UpperCaseNamingStrategy(CultureInfo culture)
        => _culture = culture;

    public string ApplyNaming(string name) => name.ToUpper(_culture);
}