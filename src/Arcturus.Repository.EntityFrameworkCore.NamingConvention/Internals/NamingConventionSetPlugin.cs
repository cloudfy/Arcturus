using Arcturus.Repository.EntityFrameworkCore.NamingConvention.NamingStrategies;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System.Globalization;

namespace Arcturus.Repository.EntityFrameworkCore.NamingConvention.Internals;

internal class NamingConventionSetPlugin(
    ProviderConventionSetBuilderDependencies dependencies
    , IDbContextOptions options) 
    : IConventionSetPlugin
{
    private readonly ProviderConventionSetBuilderDependencies _dependencies = dependencies;
    private readonly IDbContextOptions _options = options;

    public ConventionSet ModifyConventions(ConventionSet conventionSet)
    {
        var extension = _options.FindExtension<NamingConventionsOptionsExtension>() ??
            new NamingConventionsOptionsExtension().WithoutNaming();
        var namingStyle = extension.NamingConvention;
        var culture = extension.Culture;
        if (namingStyle == NamingConvention.None)
        {
            return conventionSet;
        }

        var convention = new NameRewritingConvention(_dependencies, namingStyle switch
        {
            NamingConvention.SnakeCase => new SnakeCaseNamingStrategy(culture ?? CultureInfo.InvariantCulture),
            NamingConvention.LowerCase => new LowerCaseNamingStrategy(culture ?? CultureInfo.InvariantCulture),
            NamingConvention.CamelCase => new CamelCaseNamingStrategy(culture ?? CultureInfo.InvariantCulture),
            NamingConvention.UpperCase => new UpperCaseNamingStrategy(culture ?? CultureInfo.InvariantCulture),
            NamingConvention.UpperSnakeCase => new UpperSnakeCaseNamingStrategy(culture ?? CultureInfo.InvariantCulture),
            _ => throw new ArgumentOutOfRangeException("Unhandled enum value: " + namingStyle)
        });

        conventionSet.EntityTypeAddedConventions.Add(convention);
        conventionSet.EntityTypeAnnotationChangedConventions.Add(convention);
        conventionSet.ComplexTypeAnnotationChangedConventions.Add(convention);
        conventionSet.PropertyAddedConventions.Add(convention);
        conventionSet.ForeignKeyOwnershipChangedConventions.Add(convention);
        conventionSet.KeyAddedConventions.Add(convention);
        conventionSet.ForeignKeyAddedConventions.Add(convention);
        conventionSet.ForeignKeyPropertiesChangedConventions.Add(convention);
        conventionSet.IndexAddedConventions.Add(convention);
        conventionSet.EntityTypeBaseTypeChangedConventions.Add(convention);
        conventionSet.ModelFinalizingConventions.Add(convention);

        return conventionSet;
    }
}
