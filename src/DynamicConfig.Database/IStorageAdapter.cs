using System.Threading.Tasks;
using DynamicConfig.Storage.DatabaseModel;
using StackExchange.Redis;

namespace DynamicConfig.Database {
  public interface IStorageAdapter {
    Task<DatabasePage> GetDatabasePageAsync(string key);
    Task<ServiceConfigurationPage> GetServiceConfigurationPageAsync(string applicationKey);
    Task SavePageAsync(string allRecordsKey, DatabasePage databasePage);
    Task SavePageAsync(string applicationKey, ServiceConfigurationPage serviceConfigurationPage);
  }
}