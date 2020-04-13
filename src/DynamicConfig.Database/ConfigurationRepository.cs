using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfig.Storage.DatabaseModel;
using Microsoft.Extensions.Logging;

namespace DynamicConfig.Database {
  public class ConfigurationRepository : IConfigurationRepository {
    private readonly IStorageAdapter _storageAdapter;
    private readonly ILogger<ConfigurationRepository> _logger;

    public ConfigurationRepository(IStorageAdapter storageAdapter, ILogger<ConfigurationRepository> logger) {
      _storageAdapter = storageAdapter;
      _logger = logger;
    }

    public async Task<DatabasePage> GetAllRecordsAsync(CancellationToken token) {
      return await GetDatabasePageAsync().ConfigureAwait(false);
    }

    public Task InvertStateAsync(Guid id, in CancellationToken token) {
      //not implemented yet :)
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


      databasePage.KnownRecords.Remove(serviceConfigurationRecord);

      if (serviceConfigurationRecord.IsActive) {
        var serviceConfigurationActiveRecord = serviceConfigurationPage.Entries.FirstOrDefault(e => e.Name == serviceConfigurationRecord.Name);
        serviceConfigurationPage.Entries.Remove(serviceConfigurationActiveRecord);
       await _storageAdapter.SavePageAsync(applicationKey, serviceConfigurationPage).ConfigureAwait(false);
      }

      await _storageAdapter.SavePageAsync(Constants.AllRecordsKey, databasePage).ConfigureAwait(false);
    }

    public async Task InsertAsync(ServiceConfigurationRecord record, CancellationToken token) {
      _logger.LogInformation($"Going to insert a record {record.Name}");
      record.Id = Guid.NewGuid();

      var applicationKey = record.ApplicationName.ToLowerInvariant();
      var databasePage = await GetDatabasePageAsync().ConfigureAwait(false);
      var serviceConfigurationPage = await GetServiceConfigurationPageAsync(applicationKey).ConfigureAwait(false);

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
      databasePage.KnownRecords.Add(record);
      //not a transaction btw :(
      await _storageAdapter.SavePageAsync(applicationKey, serviceConfigurationPage).ConfigureAwait(false);
      await _storageAdapter.SavePageAsync( Constants.AllRecordsKey, databasePage).ConfigureAwait(false);

    }

    private async Task<ServiceConfigurationPage> GetServiceConfigurationPageAsync(string applicationKey) =>
      await _storageAdapter.GetServiceConfigurationPageAsync(applicationKey).ConfigureAwait(false) ??
      new ServiceConfigurationPage {
        Entries = new List<ServiceConfigurationActiveRecord>()
      };

    private async Task<DatabasePage> GetDatabasePageAsync() =>
      await _storageAdapter.GetDatabasePageAsync(Constants.AllRecordsKey).ConfigureAwait(false)
      ?? new DatabasePage {
        KnownRecords = new List<ServiceConfigurationRecord>()
      };
  }
}