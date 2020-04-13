using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfig.Storage.DatabaseModel;
using DynamicConfiguration.Storage.Model;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Xunit;

namespace DynamicConfig.Database.Tests {
  public class ConfigurationRepositoryTests {
    private readonly ConfigurationRepository _configurationRepository;
    private readonly InMemoryStorageAdapter _inMemoryStorageAdapter;
    private readonly CancellationToken _token;

    public ConfigurationRepositoryTests() {
      _inMemoryStorageAdapter = new InMemoryStorageAdapter();
      _configurationRepository =
        new ConfigurationRepository(_inMemoryStorageAdapter, NullLogger<ConfigurationRepository>.Instance);
      _token = CancellationToken.None;
    }

    [Fact]
    public async Task GetAll_Returns_DatabasePage() {
      _inMemoryStorageAdapter.Db.Add(Constants.AllRecordsKey, null);
      var databasePage = await _configurationRepository.GetAllRecordsAsync(_token);
      databasePage.ShouldSatisfyAllConditions(
        () => databasePage.ShouldNotBeNull(),
        () => databasePage.KnownRecords.ShouldNotBeNull()
      );
    }

    [Fact]
    public async Task Delete_UnknownItem_Does_Not_Fail() {
      _inMemoryStorageAdapter.Db.Add(Constants.AllRecordsKey, new DatabasePage {
        KnownRecords = new List<ServiceConfigurationRecord> {new ServiceConfigurationRecord()}
      });

      await _configurationRepository.DeleteAsync(Guid.NewGuid(), _token);

      _inMemoryStorageAdapter.Db[Constants.AllRecordsKey].KnownRecords.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Delete_Removes_Record_From_Database() {
      var serviceConfigurationRecord = new ServiceConfigurationRecord {
        ApplicationName = "Service-a",
        IsActive = false,
        Id = Guid.NewGuid(),
        Name = "SettingName",
        Type = ConfigurationItemType.String,
        Value = "abc"
      };

      _inMemoryStorageAdapter.Db.Add(Constants.AllRecordsKey, new DatabasePage {
        KnownRecords = new List<ServiceConfigurationRecord> {
          serviceConfigurationRecord
        }
      });

      await _configurationRepository.DeleteAsync(serviceConfigurationRecord.Id, _token);

      _inMemoryStorageAdapter.Db[Constants.AllRecordsKey].KnownRecords.ShouldBeEmpty();
      _inMemoryStorageAdapter.DbPageToUpdate.KnownRecords.ShouldBeEmpty();
    }

    [Fact]
    public async Task Delete_ActiveRecord_Removes_Record_From_Both_Databases() {
      var serviceConfigurationRecord = new ServiceConfigurationRecord {
        ApplicationName = "Service-a",
        IsActive = true,
        Id = Guid.NewGuid(),
        Name = "SettingName",
        Type = ConfigurationItemType.String,
        Value = "abc"
      };

      _inMemoryStorageAdapter.Db.Add(Constants.AllRecordsKey, new DatabasePage {
        KnownRecords = new List<ServiceConfigurationRecord> {
          serviceConfigurationRecord
        }
      });
      _inMemoryStorageAdapter.ServiceConfigurationStorage.Add(serviceConfigurationRecord.ApplicationName.ToLowerInvariant(),
        new ServiceConfigurationPage {
          Entries = new List<ServiceConfigurationActiveRecord> {
            new ServiceConfigurationActiveRecord {
              Name = serviceConfigurationRecord.Name
            },
            new ServiceConfigurationActiveRecord {
              Name = "Settings2"
            }
          }
        });

      await _configurationRepository.DeleteAsync(serviceConfigurationRecord.Id, _token);

      _inMemoryStorageAdapter.Db[Constants.AllRecordsKey].KnownRecords.ShouldBeEmpty();
      _inMemoryStorageAdapter.DbPageToUpdate.KnownRecords.ShouldBeEmpty();

      _inMemoryStorageAdapter.ServiceConfigurationStorage[serviceConfigurationRecord.ApplicationName.ToLowerInvariant()].Entries.Count.ShouldBe(1);
      _inMemoryStorageAdapter.ServiceConfigurationStorageToUpdate.Entries.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Insert_InActive_Record() {
      var serviceConfigurationRecord = new ServiceConfigurationRecord {
        ApplicationName = "Service-a",
        IsActive = false,
        Name = "SettingName",
        Type = ConfigurationItemType.String,
        Value = "abc"
      };

      _inMemoryStorageAdapter.Db.Add(Constants.AllRecordsKey, new DatabasePage {
        KnownRecords = new List<ServiceConfigurationRecord>()
      });
      _inMemoryStorageAdapter.ServiceConfigurationStorage.Add(serviceConfigurationRecord.ApplicationName.ToLowerInvariant(),
        new ServiceConfigurationPage {
          Entries = new List<ServiceConfigurationActiveRecord> {
            new ServiceConfigurationActiveRecord {
              Name = "Settings2"
            }
          }
        });

      await _configurationRepository.InsertAsync(serviceConfigurationRecord, _token);

      _inMemoryStorageAdapter.Db[Constants.AllRecordsKey].KnownRecords.ShouldNotBeEmpty();
      _inMemoryStorageAdapter.DbPageToUpdate.KnownRecords.ShouldNotBeEmpty();

      _inMemoryStorageAdapter.ServiceConfigurationStorage[serviceConfigurationRecord.ApplicationName.ToLowerInvariant()].Entries.Count.ShouldBe(1);
      _inMemoryStorageAdapter.ServiceConfigurationStorageToUpdate.Entries.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Insert_Active_Record_Inserts_Into_Both_Databases() {
      var serviceConfigurationRecord = new ServiceConfigurationRecord {
        ApplicationName = "Service-a",
        IsActive = true,
        Name = "SettingName",
        Type = ConfigurationItemType.String,
        Value = "abc"
      };

      _inMemoryStorageAdapter.Db.Add(Constants.AllRecordsKey, new DatabasePage {
        KnownRecords = new List<ServiceConfigurationRecord>()
      });
      _inMemoryStorageAdapter.ServiceConfigurationStorage.Add(serviceConfigurationRecord.ApplicationName.ToLowerInvariant(),
        new ServiceConfigurationPage {
          Entries = new List<ServiceConfigurationActiveRecord> {
            new ServiceConfigurationActiveRecord {
              Name = "Settings2"
            }
          }
        });

      await _configurationRepository.InsertAsync(serviceConfigurationRecord, _token);

      _inMemoryStorageAdapter.Db[Constants.AllRecordsKey].KnownRecords.ShouldNotBeEmpty();
      _inMemoryStorageAdapter.DbPageToUpdate.KnownRecords.ShouldNotBeEmpty();

      _inMemoryStorageAdapter.ServiceConfigurationStorage[serviceConfigurationRecord.ApplicationName.ToLowerInvariant()].Entries.Count.ShouldBe(2);
      _inMemoryStorageAdapter.ServiceConfigurationStorageToUpdate.Entries.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Insert_Active_Record_Replaces_Existing_Active_Records() {
      var serviceConfigurationRecord = new ServiceConfigurationRecord {
        ApplicationName = "Service-a",
        IsActive = true,
        Name = "SettingName",
        Type = ConfigurationItemType.String,
        Value = "abc"
      };

      _inMemoryStorageAdapter.Db.Add(Constants.AllRecordsKey, new DatabasePage {
        KnownRecords = new List<ServiceConfigurationRecord>()
      });
      _inMemoryStorageAdapter.ServiceConfigurationStorage.Add(serviceConfigurationRecord.ApplicationName.ToLowerInvariant(),
        new ServiceConfigurationPage {
          Entries = new List<ServiceConfigurationActiveRecord> {
            new ServiceConfigurationActiveRecord {
              Name = serviceConfigurationRecord.Name,
              Value = "123"
            }
          }
        });

      await _configurationRepository.InsertAsync(serviceConfigurationRecord, _token);

      _inMemoryStorageAdapter.ServiceConfigurationStorage[serviceConfigurationRecord.ApplicationName.ToLowerInvariant()].Entries.Count.ShouldBe(1);
      _inMemoryStorageAdapter.ServiceConfigurationStorageToUpdate.Entries.Count.ShouldBe(1);
      _inMemoryStorageAdapter.ServiceConfigurationStorageToUpdate.Entries.First().Value.ShouldBe(serviceConfigurationRecord.Value);
    }
  }
}