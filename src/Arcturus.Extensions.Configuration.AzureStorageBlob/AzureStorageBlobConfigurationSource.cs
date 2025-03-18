using Arcturus.Extensions.Configuration.AzureStorageBlob.Internals;
using Microsoft.Extensions.Configuration;

namespace Arcturus.Extensions.Configuration.AzureStorageBlob;

public class AzureStorageBlobConfigurationSource : IConfigurationSource
{
    private readonly Func<AzureStorageBlobConfigurationOptions> _optionsProvider;

    internal AzureStorageBlobConfigurationSource(Action<AzureStorageBlobConfigurationOptions> optionsInitializer)
    {
        _optionsProvider = () =>
        {
            var options = new AzureStorageBlobConfigurationOptions();
            optionsInitializer(options);
            return options;
        };
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        IConfigurationProvider? provider = null;

        try
        {
            AzureStorageBlobConfigurationOptions options = _optionsProvider();
            ConfigurationClientManager clientManager;

            if (options.ConnectionString != null)
            {
                clientManager = new ConfigurationClientManager(
                    options.ConnectionString,
                    options.Container,
                    options.BlobName,
                    options);
            }
            else
            {
                throw new ArgumentException(
                    $"Please call {nameof(AzureStorageBlobConfigurationOptions)}.{nameof(AzureStorageBlobConfigurationOptions.Connect)} to specify how to connect to Azure Storage Blob.");
            }

            provider = new AzureStorageBlobConfigurationProvider(clientManager, options);
        }
        catch (InvalidOperationException ex)
        {
            throw new ArgumentException(ex.Message, ex);
        }
        catch (FormatException fe)
        {
            throw new ArgumentException(fe.Message, fe);
        }

        return provider ?? new EmptyConfigurationProvider();
    }
}
