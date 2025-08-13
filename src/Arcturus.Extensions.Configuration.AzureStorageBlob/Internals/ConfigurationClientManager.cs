using Azure.Core;
using Azure.Storage.Blobs;

namespace Arcturus.Extensions.Configuration.AzureStorageBlob.Internals;

internal class ConfigurationClientManager : IDisposable
{
    private readonly string _connectionString;
    private readonly string _container;
    private readonly string _blobName;
    private readonly AzureStorageBlobConfigurationOptions _options;

    private bool _isInitialLoadComplete = false;

    internal ConfigurationClientManager(
        string connectionString, string container, string blobName, AzureStorageBlobConfigurationOptions options)
    {
        _connectionString = connectionString;
        _container = container;
        _blobName = blobName;

        _options = options;
    }

    internal IDictionary<string, string> LoadDictionarySettings()
    {
        try
        {
            using var startupCancellationTokenSource = new CancellationTokenSource(_options.Startup.Timeout);

            // Load() is invoked only once during application startup. We don't need to check for concurrent network
            // operations here because there can't be any other startup or refresh operation in progress at this time.

            // Mark all settings have loaded at startup.
            _isInitialLoadComplete = true;

            return LoadDictionarySettingsAsync(startupCancellationTokenSource.Token)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
        catch (ArgumentException)
        {
            // Instantly re-throw the exception
            throw;
        }
        catch
        {
            throw;
        }
        finally
        {
        }
    }

    private async Task<IDictionary<string, string>> LoadDictionarySettingsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var blobClientOptions = new BlobClientOptions
            {
                Retry =
                {
                    MaxRetries = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    MaxDelay = TimeSpan.FromSeconds(30),
                    Mode = RetryMode.Exponential
                }
            };

            TokenCredential? credential = _options.GetCredential?.Invoke();
            BlobClient blobClient;

            if (credential is not null)
            {
                blobClient = new BlobClient(
                    new Uri($"{_connectionString}{_container}/{_blobName}"), credential, blobClientOptions);
            }
            else
            {
                blobClient = new BlobClient(_connectionString, _container, _blobName, blobClientOptions);
            }

            var downloadResponse = await blobClient.DownloadContentAsync(cancellationToken);
            var downloadContent = downloadResponse.Value.Content.ToString();

            var jsonSettings = System.Text.Json.JsonDocument.Parse(downloadContent);

            return JsonHelper.FlattenJson(jsonSettings);
        }
        catch (Exception e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }

    public void Dispose()
    {
        //throw new NotImplementedException();
    }
}
