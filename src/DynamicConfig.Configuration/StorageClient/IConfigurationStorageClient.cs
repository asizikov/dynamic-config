using System.Threading;
using System.Threading.Tasks;
using DynamicConfiguration.Storage.Model;

namespace DynamicConfig.Configuration.StorageClient {
  public interface IConfigurationStorageClient {
    Task<ServiceConfigurationResponse> ExecuteAsync(string applicationName, CancellationToken token);
  }
}