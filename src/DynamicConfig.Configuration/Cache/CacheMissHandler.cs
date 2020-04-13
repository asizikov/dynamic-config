using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DynamicConfig.Configuration.Cache {
  public class DefaultCacheMissHandler : ICacheMissHandler {
    private readonly ILogger<DefaultCacheMissHandler> _logger;
    private readonly IConfiguration _configuration;

    public DefaultCacheMissHandler(ILogger<DefaultCacheMissHandler> logger, IConfiguration configuration) {
      _logger = logger;
      _configuration = configuration;
    }

    public TType HandleCacheMiss<TType>(string key) {
      _logger.LogInformation($"Going to handle cache miss for {key}");
      var configurationSection = _configuration.GetSection(key);
      if (configurationSection.Exists()) {
        _logger.LogInformation($"Found default (static) value for {key}");
        return _configuration.GetValue<TType>(key);
      }

      throw new InvalidOperationException($"Unknown config key {key} requested for type {typeof(TType)}");
    }
  }
}