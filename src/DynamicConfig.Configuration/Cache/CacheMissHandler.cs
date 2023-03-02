using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DynamicConfig.Configuration.Cache {
  public class DefaultCacheMissHandler : ICacheMissHandler {
    private readonly ILogger<DefaultCacheMissHandler> _logger;
    private readonly IConfiguration _configuration;

    public DefaultCacheMissHandler(ILogger<DefaultCacheMissHandler> logger, IConfiguration configuration) {
      // ensure that params are not null and assign to fileds
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));      
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