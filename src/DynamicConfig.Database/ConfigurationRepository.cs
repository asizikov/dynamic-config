using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfig.Storage.DatabaseModel;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace DynamicConfig.Database {
  public class ConfigurationRepository : IConfigurationRepository {
    private readonly IRedisCacheClient _redis;
    private readonly ILogger<ConfigurationRepository> _logger;
    private const string AllRecordsKey = "KnownConfigurations";

    public ConfigurationRepository(IRedisCacheClient redis, ILogger<ConfigurationRepository> logger) {
      _redis = redis;
      _logger = logger;
    }

    public async Task<DatabasePage> GetAllRecordsAsync(CancellationToken token) {
      return await GetDatabasePageAsync().ConfigureAwait(false);
    }

    public Task InvertStateAsync(Guid id, in CancellationToken token) {
      return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken token) {
      _logger.LogInformation($"Going to delete a record with ID: {id}");
      var databasePage = await GetDatabasePageAsync().ConfigureAwait(false);
      var serviceConfigurationRecord = databasePage.KnownRecords.FirstOrDefault(i => i.Id == id);
      if (serviceConfigurationRecord is null) {
        _logger.LogWarning($"Attempt to delete an unknown record with id: {id}");
        return;
      }

      var applicationKey = serviceConfigurationRecord.ApplicationName.ToLowerInvariant();
      var serviceConfigurationPage = await GetServiceConfigurationPageAsync(applicationKey).ConfigureAwait(false);

      var transaction = _redis.Db0.Database.CreateTransaction();
      databasePage.KnownRecords.Remove(serviceConfigurationRecord);

      if (serviceConfigurationRecord.IsActive) {
        var serviceConfigurationActiveRecord = serviceConfigurationPage.Entries.FirstOrDefault(e => e.Name == serviceConfigurationRecord.Name);
        serviceConfigurationPage.Entries.Remove(serviceConfigurationActiveRecord);

#pragma warning disable 4014
        transaction.StringSetAsync(applicationKey, JsonConvert.SerializeObject(serviceConfigurationPage));
#pragma warning restore 4014
      }
#pragma warning disable 4014
      transaction.StringSetAsync(AllRecordsKey, JsonConvert.SerializeObject(databasePage));
#pragma warning restore 4014

      var result = await transaction.ExecuteAsync().ConfigureAwait(false);
      if (!result) {
        _logger.LogWarning("failed to submit transaction");
      }
    }

    public async Task InsertAsync(ServiceConfigurationRecord record, CancellationToken token) {
      _logger.LogInformation($"Going to submit a record {record.Name}");
      record.Id = Guid.NewGuid();
      var applicationKey = record.ApplicationName.ToLowerInvariant();

      var databasePage = await GetDatabasePageAsync().ConfigureAwait(false);
      var serviceConfigurationPage = await GetServiceConfigurationPageAsync(applicationKey).ConfigureAwait(false);

      var transaction = _redis.Db0.Database.CreateTransaction();

      databasePage.KnownRecords.Add(record);
      var duplicate = serviceConfigurationPage.Entries.FirstOrDefault(r => r.Name == record.Name);
      if (duplicate != null) {
        serviceConfigurationPage.Entries.Remove(duplicate);
      }

      if (record.IsActive) {
        var existing = serviceConfigurationPage.Entries.FirstOrDefault(r => r.Name == record.Name);
        if (existing != null) {
          serviceConfigurationPage.Entries.Remove(existing);
        }

        serviceConfigurationPage.Entries.Add(new ServiceConfigurationActiveRecord {
          Name = record.Name,
          Type = record.Type,
          Value = record.Value
        });
        foreach (var databasePageKnownRecord in databasePage.KnownRecords
          .Where(r => r.IsActive && r.Name == record.Name &&
                      string.Equals(r.ApplicationName, record.ApplicationName, StringComparison.InvariantCultureIgnoreCase))) {
          databasePageKnownRecord.IsActive = false;
        }
      }

#pragma warning disable 4014
      transaction.StringSetAsync(applicationKey, JsonConvert.SerializeObject(serviceConfigurationPage));
      transaction.StringSetAsync(AllRecordsKey, JsonConvert.SerializeObject(databasePage));
#pragma warning restore 4014

      var result = await transaction.ExecuteAsync().ConfigureAwait(false);
      if (!result) {
        _logger.LogWarning("failed to submit transaction");
      }
    }

    private async Task<ServiceConfigurationPage> GetServiceConfigurationPageAsync(string applicationKey) =>
      await _redis.Db0.GetAsync<ServiceConfigurationPage>(applicationKey).ConfigureAwait(false) ??
      new ServiceConfigurationPage {
        Entries = new List<ServiceConfigurationActiveRecord>()
      };

    private async Task<DatabasePage> GetDatabasePageAsync() =>
      await _redis.Db0.GetAsync<DatabasePage>(AllRecordsKey).ConfigureAwait(false) ?? new DatabasePage {
        KnownRecords = new List<ServiceConfigurationRecord>()
      };
  }
}