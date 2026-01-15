using Arcturus.Repository.EntityFrameworkCore.NamingConvention.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Globalization;

namespace Arcturus.Repository.EntityFrameworkCore.NamingConvention;

public static class NamingConventionsExtensions
{
    /// <summary>
    /// Configures the model to use the specified naming convention for database identifiers such as table and column
    /// names.
    /// </summary>
    /// <remarks>This method enables consistent naming of database objects according to the specified
    /// convention, which can help ensure compatibility with existing database schemas or organizational
    /// standards.</remarks>
    /// <param name="optionsBuilder">The options builder to configure. Must not be null.</param>
    /// <param name="convention">The naming convention to apply to database identifiers.</param>
    /// <param name="culture">The culture to use for case conversions, or null to use the current culture.</param>
    /// <returns>The same <see cref="DbContextOptionsBuilder"/> instance so that additional configuration calls can be chained.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="convention"/> is not a valid value of the <see cref="NamingConvention"/> enumeration.</exception>
    public static DbContextOptionsBuilder UseNamingConvention(
        this DbContextOptionsBuilder optionsBuilder
        , NamingConvention convention
        , CultureInfo? culture = null)
    {
        return convention switch
        {
            NamingConvention.SnakeCase => optionsBuilder.UseSnakeCaseNamingConvention(culture),
            NamingConvention.LowerCase => optionsBuilder.UseLowerCaseNamingConvention(culture),
            NamingConvention.UpperCase => optionsBuilder.UseUpperCaseNamingConvention(culture),
            NamingConvention.UpperSnakeCase => optionsBuilder.UseUpperSnakeCaseNamingConvention(culture),
            NamingConvention.CamelCase => optionsBuilder.UseCamelCaseNamingConvention(culture),
            _ => throw new ArgumentOutOfRangeException(nameof(convention), convention, null)
        };
    }

    private static DbContextOptionsBuilder UseSnakeCaseNamingConvention(
        this DbContextOptionsBuilder optionsBuilder
        , CultureInfo? culture = null)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder, nameof(optionsBuilder));

        var extension = (optionsBuilder.Options.FindExtension<NamingConventionsOptionsExtension>()
                ?? new NamingConventionsOptionsExtension())
            .WithSnakeCaseNamingConvention(culture);

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        return optionsBuilder;
    }

    private static DbContextOptionsBuilder<TContext> UseSnakeCaseNamingConvention<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder, CultureInfo? culture = null)
        where TContext : DbContext
        => (DbContextOptionsBuilder<TContext>)UseSnakeCaseNamingConvention((DbContextOptionsBuilder)optionsBuilder, culture);

    private static DbContextOptionsBuilder UseLowerCaseNamingConvention(
        this DbContextOptionsBuilder optionsBuilder,
        CultureInfo? culture = null)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder, nameof(optionsBuilder));

        var extension = (optionsBuilder.Options.FindExtension<NamingConventionsOptionsExtension>()
                ?? new NamingConventionsOptionsExtension())
            .WithLowerCaseNamingConvention(culture);

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        return optionsBuilder;
    }

    private static DbContextOptionsBuilder<TContext> UseLowerCaseNamingConvention<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder,
        CultureInfo? culture = null)
        where TContext : DbContext
        => (DbContextOptionsBuilder<TContext>)UseLowerCaseNamingConvention((DbContextOptionsBuilder)optionsBuilder, culture);

    private static DbContextOptionsBuilder UseUpperCaseNamingConvention(
        this DbContextOptionsBuilder optionsBuilder,
        CultureInfo? culture = null)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder, nameof(optionsBuilder));
        
        var extension = (optionsBuilder.Options.FindExtension<NamingConventionsOptionsExtension>()
                ?? new NamingConventionsOptionsExtension())
            .WithUpperCaseNamingConvention(culture);

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        return optionsBuilder;
    }

    private static DbContextOptionsBuilder<TContext> UseUpperCaseNamingConvention<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder,
        CultureInfo? culture = null)
        where TContext : DbContext
        => (DbContextOptionsBuilder<TContext>)UseUpperCaseNamingConvention((DbContextOptionsBuilder)optionsBuilder, culture);

    private static DbContextOptionsBuilder UseUpperSnakeCaseNamingConvention(
        this DbContextOptionsBuilder optionsBuilder,
        CultureInfo? culture = null)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder, nameof(optionsBuilder));

        var extension = (optionsBuilder.Options.FindExtension<NamingConventionsOptionsExtension>()
                ?? new NamingConventionsOptionsExtension())
            .WithUpperSnakeCaseNamingConvention(culture);

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        return optionsBuilder;
    }

    private static DbContextOptionsBuilder<TContext> UseUpperSnakeCaseNamingConvention<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder,
        CultureInfo? culture = null)
        where TContext : DbContext
        => (DbContextOptionsBuilder<TContext>)UseUpperSnakeCaseNamingConvention((DbContextOptionsBuilder)optionsBuilder, culture);

    private static DbContextOptionsBuilder UseCamelCaseNamingConvention(
        this DbContextOptionsBuilder optionsBuilder,
        CultureInfo? culture = null)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder, nameof(optionsBuilder));

        var extension = (optionsBuilder.Options.FindExtension<NamingConventionsOptionsExtension>()
                ?? new NamingConventionsOptionsExtension())
            .WithCamelCaseNamingConvention(culture);

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        return optionsBuilder;
    }

    private static DbContextOptionsBuilder<TContext> UseCamelCaseNamingConvention<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder,
        CultureInfo? culture = null)
        where TContext : DbContext
        => (DbContextOptionsBuilder<TContext>)UseCamelCaseNamingConvention((DbContextOptionsBuilder)optionsBuilder, culture);
}