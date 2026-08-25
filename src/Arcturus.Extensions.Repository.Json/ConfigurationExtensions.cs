using Arcturus.Repository.Json.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;
using System.Text.Json;

namespace Arcturus.Extensions.Repository.Json;

/// <summary>
/// Provides extension methods for configuring entity properties with JSON serialization and custom conversions in Entity Framework Core.
/// </summary>
public static class ConfigurationExtensions
{
    [Obsolete("Use PropertyColumnConversion instead.")]
    public static PropertyBuilder<TProperty> ConfigureColumnConversion<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TProperty>> propertyExpression,
        string columnName,
        Func<string, TProperty> toProvider
        , string columnType = "jsonb")
        where TEntity : class
        => PropertyColumnConversion<TEntity, TProperty>(builder, propertyExpression, columnName, toProvider, columnType);
    [Obsolete("Use PropertyReadOnlyJsonCollection instead.")]
    public static PropertyBuilder<IReadOnlyCollection<TProperty>> ConfigureReadOnlyJsonCollection<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, IReadOnlyCollection<TProperty>>> propertyExpression
        , string columnName
        , string columnType = "jsonb")
        where TEntity : class
        => PropertyReadOnlyJsonCollection<TEntity, TProperty>(builder, propertyExpression, columnName, columnType);
    [Obsolete("Use PropertyJsonProperty instead.")]
    public static PropertyBuilder<TProperty> ConfigureJsonProperty<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TProperty>> propertyExpression,
        string columnName
        , string columnType = "jsonb")
        where TEntity : class
        => PropertyJsonProperty<TEntity, TProperty>(builder, propertyExpression, columnName, columnType);
    [Obsolete("Use PropertyJsonProperty instead.")]
    public static PropertyBuilder<ICollection<TProperty>> ConfigureJsonCollection<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, ICollection<TProperty>>> propertyExpression
        , string columnName
        , string columnType = "jsonb")
        where TEntity : class
        => PropertyJsonCollection<TEntity, TProperty>(builder, propertyExpression, columnName, columnType);
    [Obsolete("Use PropertyJsonList instead.")]
    public static PropertyBuilder<IList<TProperty>> ConfigureJsonList<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, IList<TProperty>>> propertyExpression
        , string columnName
        , string columnType = "jsonb")
        where TEntity : class
        => PropertyJsonList<TEntity, TProperty>(builder, propertyExpression, columnName, columnType);

    /// <summary>
    /// Configures a property with a custom conversion for the specified entity type.
    /// The property is stored as a column in the database and is converted to and from a string.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="builder">The entity type builder.</param>
    /// <param name="propertyExpression">An expression representing the property to configure.</param>
    /// <param name="columnName">The name of the column in the database.</param>
    /// <param name="toProvider">A function to convert the string value from the database to the property type.</param>
    /// <param name="columnType">The type of the column in the database. Default jsonb.</param>
    /// <returns>A <see cref="PropertyBuilder{TProperty}"/> for further configuration.</returns>
    /// <remarks>Require: Support nullable columns.</remarks>
    public static PropertyBuilder<TProperty> PropertyColumnConversion<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TProperty>> propertyExpression,
        string columnName,
        Func<string, TProperty> toProvider
        , string columnType = "jsonb")
        where TEntity : class
    {
        return builder
            .Property(propertyExpression)
            .HasColumnName(columnName)
            .HasColumnType(columnType)
            .HasConversion(
                v => v == null || v.Equals(default(TProperty)) ? null : v.ToString(),
                v => string.IsNullOrWhiteSpace(v) ? default! : toProvider(v));
    }

    /// <summary>
    /// Configures a read-only JSON collection property for the specified entity type.
    /// The property is stored as a JSONB column in the database and is deserialized into a read-only collection.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TProperty">The type of the collection elements.</typeparam>
    /// <param name="builder">The entity type builder.</param>
    /// <param name="propertyExpression">An expression representing the property to configure.</param>
    /// <param name="columnName">The name of the column in the database.</param>
    /// <param name="columnType">The type of the column in the database. Default jsonb.</param>
    /// <returns>A <see cref="PropertyBuilder{TProperty}"/> for further configuration.</returns>
    /// <remarks>Require: Support nullable columns.</remarks>
    public static PropertyBuilder<IReadOnlyCollection<TProperty>> PropertyReadOnlyJsonCollection<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, IReadOnlyCollection<TProperty>>> propertyExpression
        , string columnName
        , string columnType = "jsonb")
        where TEntity : class
    {
        return builder.Property(propertyExpression)
            .HasColumnName(columnName)
            .HasColumnType(columnType)
            .HasConversion(
                v => v == null || v.Count == 0 ? null : JsonSerializer.Serialize(
                    v
                    , SpecificEfJsonSerializer.GetJsonOptions()),
                json => string.IsNullOrWhiteSpace(json)
                    ? new List<TProperty>()
                    : JsonSerializer.Deserialize<List<TProperty>>(
                        json
                        , SpecificEfJsonSerializer.GetJsonOptions()) ?? new List<TProperty>()
            );
    }
    /// <summary>
    /// Configures a property with a JSON conversion for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="builder"></param>
    /// <param name="propertyExpression"></param>
    /// <param name="columnName">The name of the column in the database.</param>
    /// <param name="columnType">The type of the column in the database. Default jsonb.</param>
    /// <returns>A <see cref="PropertyBuilder{TProperty}"/> for further configuration.</returns>
    /// <remarks>Require: Support nullable columns.</remarks>
    public static PropertyBuilder<TProperty> PropertyJsonProperty<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TProperty>> propertyExpression,
        string columnName
        , string columnType = "jsonb")
        where TEntity : class
    {
        return builder.Property(propertyExpression)
            .HasColumnName(columnName)
            .HasColumnType(columnType)
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, SpecificEfJsonSerializer.GetJsonOptions()),
                json => string.IsNullOrWhiteSpace(json)
                    ? default!
                    : JsonSerializer.Deserialize<TProperty>(
                        json
                        , SpecificEfJsonSerializer.GetJsonOptions()) ?? default!);
    }
    /// <summary>
    /// Configures a property with a JSON conversion for the specified entity type, specifically for properties that are collections.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="builder"></param>
    /// <param name="propertyExpression"></param>
    /// <param name="columnName">The name of the column in the database.</param>
    /// <param name="columnType">The type of the column in the database. Default jsonb.</param>
    /// <returns>A <see cref="PropertyBuilder{TProperty}"/> for further configuration.</returns>
    /// <remarks>Require: Support nullable columns.</remarks>
    public static PropertyBuilder<ICollection<TProperty>> PropertyJsonCollection<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, ICollection<TProperty>>> propertyExpression
        , string columnName
        , string columnType = "jsonb")
        where TEntity : class
    {
        return builder.Property(propertyExpression)
            .HasColumnName(columnName)
            .HasColumnType(columnType)
            .HasConversion(
                v => v == null || v.Count == 0 ? null : JsonSerializer.Serialize(
                    v
                    , SpecificEfJsonSerializer.GetJsonOptions()),
                json => string.IsNullOrWhiteSpace(json)
                    ? new List<TProperty>()
                    : JsonSerializer.Deserialize<List<TProperty>>(
                        json
                        , SpecificEfJsonSerializer.GetJsonOptions()) ?? new List<TProperty>()
            );
    }
    /// <summary>
    /// Configures a property with a JSON conversion for the specified entity type, specifically for properties that are lists.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="builder"></param>
    /// <param name="propertyExpression"></param>
    /// <param name="columnName">The name of the column in the database.</param>
    /// <param name="columnType">The type of the column in the database. Default jsonb.</param>
    /// <returns>A <see cref="PropertyBuilder{TProperty}"/> for further configuration.</returns>
    /// <remarks>Require: Support nullable columns.</remarks>
    public static PropertyBuilder<IList<TProperty>> PropertyJsonList<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, IList<TProperty>>> propertyExpression
        , string columnName
        , string columnType = "jsonb")
        where TEntity : class
    {
        return builder.Property(propertyExpression)
            .HasColumnName(columnName)
            .HasColumnType(columnType)
            .HasConversion(
                v => v == null || v.Count == 0 ? null : JsonSerializer.Serialize(
                    v
                    , SpecificEfJsonSerializer.GetJsonOptions()),
                json => string.IsNullOrWhiteSpace(json)
                    ? new List<TProperty>()
                    : JsonSerializer.Deserialize<List<TProperty>>(
                        json
                        , SpecificEfJsonSerializer.GetJsonOptions()) ?? new List<TProperty>()
            );
    }
}