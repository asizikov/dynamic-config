using System;
using System.Net.Http;
using System.Threading;
using DynamicConfig.Configuration.Abstractions;
using DynamicConfig.Configuration.BackgroundProcess;
using DynamicConfig.Configuration.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DynamicConfig.Configuration {
  internal class AdvancedConfigurationReader : IConfigurationReader {
    private readonly ILogger<AdvancedConfigurationReader> _logger;
    private readonly ISettingsCacheResolver _settingsCacheResolver;
    private readonly ICacheMissHandler _cacheMissHandler;
    private readonly IPeriodicJobRunner _periodicJobRunner;

    public AdvancedConfigurationReader(ILogger<AdvancedConfigurationReader> logger, ISettingsCacheResolver settingsCacheResolver,
      ICacheMissHandler cacheMissHandler, IPeriodicJobRunner periodicJobRunner) {
      _logger = logger;
      _settingsCacheResolver = settingsCacheResolver;
      _cacheMissHandler = cacheMissHandler;
      _periodicJobRunner = periodicJobRunner;
      _periodicJobRunner.Start();
    }

    public T GetValue<T>(string key) {
      _logger.LogInformation($"{key} requested");
      var cache = _settingsCacheResolver.ResolveSettingCache<T>();
      return cache.GetSetting(key, _cacheMissHandler);
    }
  }
}