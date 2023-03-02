using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfig.Configuration.Cache;
using DynamicConfig.Configuration.StorageClient;
using DynamicConfiguration.Storage.Model;
using Microsoft.Extensions.Logging;

namespace DynamicConfig.Configuration.BackgroundProcess {
  public class ConfigurationUpdaterJob : IConfigurationUpdaterJob {
    private readonly ISettingsCacheManager _settingsCacheManager;
    private readonly IConfigurationStorageClient _configurationStorageClient;
    private readonly ILogger<ConfigurationUpdaterJob> _logger;

    public ConfigurationUpdaterJob(ISettingsCacheManager settingsCacheManager, IConfigurationStorageClient configurationStorageClient,
      ILogger<ConfigurationUpdaterJob> logger) {
      _settingsCacheManager = settingsCacheManager;
      _configurationStorageClient = configurationStorageClient;
      _logger = logger;
    }

    public async Task ExecuteAsync(string applicationName, CancellationToken token) {
      try {
        _logger.LogInformation("Going to request updated configuration");
        var serviceConfigurationResponse = await _configurationStorageClient.ExecuteAsync(applicationName, token).ConfigureAwait(false);
        _logger.LogInformation("Received an up-to-date config");
        ProcessResponseAndUpdateConfig(serviceConfigurationResponse);
        _logger.LogInformation("Updated settings cache");
      }
      catch (Exception e) {
        _logger.LogError(e, "Failed to retrieve of process an up-to-date configuration for application. Will try again later");
      }
    }

    private void ProcessResponseAndUpdateConfig(ServiceConfigurationResponse serviceConfigurationResponse) {
      var ints = new Dictionary<string, Setting<int>>();
      var booleans = new Dictionary<string, Setting<bool>>();
      var strings = new Dictionary<string, Setting<string>>();
      
      foreach (var item in serviceConfigurationResponse.ConfigurationItems) {
        switch (item.Type) {
          case ConfigurationItemType.Boolean:
            booleans.Add(item.Name, new Setting<bool>(bool.Parse(item.Value)));
            break;
          case ConfigurationItemType.String:
            strings.Add(item.Name, new Setting<string>(item.Value));
            break;
          case ConfigurationItemType.Integer:
            ints.Add(item.Name, new Setting<int>(int.Parse(item.Value)));
            break;
          default:
            _logger.LogWarning($"unknown type {item.Type}");
            break;
        }
      }

      _settingsCacheManager.ReloadSettingsCache(new SettingsCache<int>(ints), new SettingsCache<bool>(booleans), new SettingsCache<string>(strings));
    }
  }
}