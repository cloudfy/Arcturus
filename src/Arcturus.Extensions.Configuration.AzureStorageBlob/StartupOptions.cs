namespace Arcturus.Extensions.Configuration.AzureStorageBlob;

public class StartupOptions
{
    /// <summary>
    /// The amount of time allowed to load data from Azure Storage Blob. Default is 100 second(s).
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);
}