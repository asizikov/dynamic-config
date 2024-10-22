using System.Threading.Tasks;
using DynamicConfig.Storage.DatabaseModel;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace DynamicConfig.Database {
  public class StorageAdapter : IStorageAdapter {
    private readonly IRedisClient _redis;
    private readonly IDistributedCache _distributedCache;

    public StorageAdapter(IRedisClient redis, IDistributedCache distributedCache) {
      _redis = redis;
      _distributedCache = distributedCache;
    }

    public Task<DatabasePage> GetDatabasePageAsync(string key) =>
      _redis.Db0.GetAsync<DatabasePage>(Constants.AllRecordsKey);

    public async Task<ServiceConfigurationPage> GetServiceConfigurationPageAsync(string applicationKey) {
      var json = await _distributedCache.GetStringAsync(applicationKey);
      return json is null ? null : JsonConvert.DeserializeObject<ServiceConfigurationPage>(json);
    }
    public Task SavePageAsync(string allRecordsKey, DatabasePage databasePage)
      => _redis.Db0.AddAsync(allRecordsKey, databasePage);

    public Task SavePageAsync(string applicationKey, ServiceConfigurationPage serviceConfigurationPage)
      => _distributedCache.SetStringAsync(applicationKey, JsonConvert.SerializeObject(serviceConfigurationPage));
  }
}