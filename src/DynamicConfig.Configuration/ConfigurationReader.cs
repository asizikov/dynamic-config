using System;
using System.Net.Http;
using DynamicConfig.Configuration.Abstractions;
using DynamicConfig.Configuration.BackgroundProcess;
using DynamicConfig.Configuration.Cache;
using DynamicConfig.Configuration.StorageClient;
using Microsoft.Extensions.Logging.Abstractions;

namespace DynamicConfig.Configuration {
  public class ConfigurationReader : IConfigurationReader {
    private readonly IConfigurationReader _advancedConfigurationReader;

    public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs) {
      var settingsCacheManager = new SettingsCacheManager();

      _advancedConfigurationReader = new AdvancedConfigurationReader(
        NullLogger<AdvancedConfigurationReader>.Instance, settingsCacheManager,
        new EmptyCacheMissHandler(),
        new PeriodicJobRunner(NullLogger<PeriodicJobRunner>.Instance,
          new ConfigurationUpdaterJob(settingsCacheManager,
            new ConfigurationStorageClient(NullLogger<ConfigurationStorageClient>.Instance,
              new HttpClient {BaseAddress = new Uri(connectionString)}),
            NullLogger<ConfigurationUpdaterJob>.Instance), applicationName,
          refreshTimerIntervalInMs));
    }

    public T GetValue<T>(string key) => _advancedConfigurationReader.GetValue<T>(key);
  }
}