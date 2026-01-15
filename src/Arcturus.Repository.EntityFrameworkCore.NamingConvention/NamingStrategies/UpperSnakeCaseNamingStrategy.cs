using System.Globalization;

namespace Arcturus.Repository.EntityFrameworkCore.NamingConvention.NamingStrategies;

public sealed class UpperSnakeCaseNamingStrategy : SnakeCaseNamingStrategy
{
    private readonly CultureInfo _culture;

    internal UpperSnakeCaseNamingStrategy(CultureInfo culture) : base(culture)
        => _culture = culture;

    public override string ApplyNaming(string name) => base.ApplyNaming(name).ToUpper(_culture);
}