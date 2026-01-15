using Arcturus.Repository.EntityFrameworkCore.NamingConvention.Abstracts;
using System.Globalization;

namespace Arcturus.Repository.EntityFrameworkCore.NamingConvention.NamingStrategies;

public class CamelCaseNamingStrategy : INamingStrategy
{
    private readonly CultureInfo _culture;

    internal CamelCaseNamingStrategy(CultureInfo culture) => _culture = culture;

    public string ApplyNaming(string name) =>
        string.IsNullOrEmpty(name)
            ? name
            : name.Length == 1
                ? char.ToLower(name[0], _culture).ToString()
                : char.ToLower(name[0], _culture) + name.Substring(1);
}
