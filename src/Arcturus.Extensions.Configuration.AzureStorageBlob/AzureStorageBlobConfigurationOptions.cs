namespace Arcturus.Extensions.Configuration.AzureStorageBlob;

public class AzureStorageBlobConfigurationOptions
{
    /// <summary>
    /// Gets the connection string for the Azure Storage Blob.
    /// </summary>
    public string ConnectionString { get; internal set; } = null!;
    /// <summary>
    /// Gets the container of the <see cref="BlobName"/> for the Azure Storage Blob.
    /// </summary>
    public string Container { get; internal set; } = null!;
    /// <summary>
    /// Gets the blob name for the Azure Storage Blob ex. appsettings.json.
    /// </summary>
    public string BlobName { get; internal set; } = null!;
    /// <summary>
    /// Gets or set the function to get the <see cref="Azure.Core.TokenCredential"/> for the Azure Storage Blob.
    /// </summary>
    /// <remarks>
    /// When assigning the function, the <see cref="ConnectionString"/> must be assigned the ServiceUrl of the Azure Storage Blob, ex. <i>"https://{account_name}.blob.core.windows.net"</i>.
    /// </remarks>
    public Func<Azure.Core.TokenCredential>? GetCredential { get; internal set; }

    /// <summary>
    /// Connects to the Azure Storage Blob with the connection string, container name, and blob name.
    /// </summary>
    /// <param name="connectionString">Connection string of Azure Storage Blob.</param>
    /// <param name="containerName">Container of the <paramref name="blobName"/>.</param>
    /// <param name="blobName">Blob file ex. appsettings.json.</param>
    /// <returns><see cref="AzureStorageBlobConfigurationOptions"/></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public AzureStorageBlobConfigurationOptions Connect(
        string connectionString, string containerName, string blobName)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }
        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new ArgumentNullException(nameof(containerName));
        }
        if (string.IsNullOrWhiteSpace(blobName))
        {
            throw new ArgumentNullException(nameof(blobName));
        }

        ConnectionString = connectionString;
        Container = containerName;
        BlobName = blobName;

        return this;
    }
    /// <summary>
    /// Connects to the Azure Storage Blob using Service Uri, container name, blob name, and function to get the <see cref="Azure.Core.TokenCredential"/>.
    /// </summary>
    /// <param name="serviceUri">ServiceUrl of the Azure Storage Blob, ex. <i>"https://{account_name}.blob.core.windows.net"</i>.</param>
    /// <param name="containerName">Container of the <paramref name="blobName"/>.</param>
    /// <param name="blobName">Blob file ex. appsettings.json.</param>
    /// <param name="getCredential">Function to return <see cref="Azure.Core.TokenCredential"/>.</param>
    /// <returns><see cref="AzureStorageBlobConfigurationOptions"/></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public AzureStorageBlobConfigurationOptions Connect(
        Uri serviceUri, string containerName, string blobName, Func<Azure.Core.TokenCredential> getCredential)
    {
        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new ArgumentNullException(nameof(containerName));
        }
        if (string.IsNullOrWhiteSpace(blobName))
        {
            throw new ArgumentNullException(nameof(blobName));
        }

        GetCredential = getCredential;
        ConnectionString = serviceUri.ToString();
        Container = containerName;
        BlobName = blobName;

        return this;
    }

    internal StartupOptions Startup { get; set; } = new StartupOptions();
}