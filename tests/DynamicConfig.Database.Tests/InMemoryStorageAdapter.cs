using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicConfig.Storage.DatabaseModel;
using Moq;
using StackExchange.Redis;

namespace DynamicConfig.Database.Tests {
  public class InMemoryStorageAdapter : IStorageAdapter {
    public readonly Dictionary<string, DatabasePage> Db = new Dictionary<string, DatabasePage>();
    public readonly Dictionary<string, ServiceConfigurationPage> ServiceConfigurationStorage = new Dictionary<string, ServiceConfigurationPage>();

    public DatabasePage DbPageToUpdate { get; set; }
    public ServiceConfigurationPage ServiceConfigurationStorageToUpdate { get; set; }

    public Task<DatabasePage> GetDatabasePageAsync(string key) => Task.FromResult(Db[key]);

    public Task<ServiceConfigurationPage> GetServiceConfigurationPageAsync(string applicationKey) {
      if (ServiceConfigurationStorage.ContainsKey(applicationKey)) {
        return Task.FromResult(ServiceConfigurationStorage[applicationKey]);
      }

      return Task.FromResult<ServiceConfigurationPage>(null);
    }


    public Task SavePageAsync(string allRecordsKey, DatabasePage databasePage) {
      DbPageToUpdate = databasePage;
      return Task.CompletedTask;
    }

    public Task SavePageAsync(string applicationKey, ServiceConfigurationPage serviceConfigurationPage) {
       ServiceConfigurationStorageToUpdate = serviceConfigurationPage;
       return Task.CompletedTask;
    }
  }
}