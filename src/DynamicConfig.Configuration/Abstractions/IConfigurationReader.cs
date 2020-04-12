using System.Threading;
using System.Threading.Tasks;

namespace DynamicConfig.Configuration.Abstractions {
  public interface IConfigurationReader {
    Task<T> GetValueAsync<T>(string key, CancellationToken token);
  }
}