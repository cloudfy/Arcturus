using Microsoft.Extensions.Configuration;

namespace Arcturus.Extensions.Configuration.AzureStorageBlob;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds an Azure Storage Blob configuration source to a configuration builder.
    /// </summary>
    /// <param name="builder">The configuration builder to add key-values to.</param>
    /// <param name="connectionString">The connection string used to connect to the configuration store.</param>
    /// <param name="containerName">The container storign the configuration <paramref name="blobName"/>.</param>
    /// <param name="blobName">The name of the configuration file, ex. appsettings.json.</param>
    /// <returns>The provided configuration builder.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"/> will always be thrown when the caller gives an invalid input configuration (connection string etc).
    public static IConfigurationBuilder AddAzureStorageBlob(
        this IConfigurationBuilder builder
        , string connectionString
        , string containerName
        , string blobName)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddAzureStorageBlob(options => options.Connect(connectionString, containerName, blobName));
    }
    /// <summary>
    /// Adds an Azure Storage Blob configuration source to a configuration builder.
    /// </summary>
    /// <param name="configurationBuilder">The configuration builder to add key-values to.</param>
    /// <param name="action"></param>
    /// <returns>The provided configuration builder.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"/> will always be thrown when the caller gives an invalid input configuration (connection string etc).
    public static IConfigurationBuilder AddAzureStorageBlob(
        this IConfigurationBuilder configurationBuilder
        , Action<AzureStorageBlobConfigurationOptions> action)
    {
        configurationBuilder.Add(new AzureStorageBlobConfigurationSource(action));

        return configurationBuilder;
    }
}
