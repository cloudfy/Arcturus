using Arcturus.Extensions.Configuration.AzureStorageBlob.Internals;
using Microsoft.Extensions.Configuration;

namespace Arcturus.Extensions.Configuration.AzureStorageBlob;

internal sealed class AzureStorageBlobConfigurationProvider
    : ConfigurationProvider, IDisposable

{
    private readonly ConfigurationClientManager _clientManager;
    private readonly AzureStorageBlobConfigurationOptions _options;

    internal AzureStorageBlobConfigurationProvider(
        ConfigurationClientManager clientManager, AzureStorageBlobConfigurationOptions options)
    {
        _clientManager = clientManager;
        _options = options;
    }

    public override void Load()
    {
        var loadedData = _clientManager.LoadDictionarySettings();
        if (loadedData is not null)
        {
            Data = loadedData!;
        }
        else
        {
            base.Load();
        }
    }

    public void Dispose()
    {
        (_clientManager as ConfigurationClientManager)?.Dispose();
    }
}