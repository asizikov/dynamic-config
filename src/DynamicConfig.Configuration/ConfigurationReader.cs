using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfig.Configuration.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DynamicConfig.Configuration {
  public class ConfigurationReader : IConfigurationReader {
    private Timer _timer;
    private readonly ILogger<ConfigurationReader> _logger;
    private readonly string _applicationName;
    private readonly string _connectionString;
    private readonly int _refreshTimerIntervalInMs;

    public ConfigurationReader(IConfiguration configuration, ILogger<ConfigurationReader> logger) {
      _logger = logger;
      _applicationName = configuration.GetSection("DYNAMIC_CONFIG_APPLICATION_NAME").Value;
      var refreshInterval = configuration.GetSection("DYNAMIC_CONFIG_REFRESH_INTERVAL_MS").Value;
      _connectionString = configuration.GetSection("DYNAMIC_CONFIG_STORAGE_CONNECTION_STRING").Value;
      _refreshTimerIntervalInMs = int.Parse(refreshInterval);
      StartConfigurationSyncProcess();
    }

    private void StartConfigurationSyncProcess() {
      _timer = new Timer(DoWork, null, TimeSpan.Zero,
        TimeSpan.FromMilliseconds(_refreshTimerIntervalInMs));
    }

    private void DoWork(object? state) {
      var http = new HttpClient{BaseAddress = new Uri(_connectionString)};
      var httpResponseMessage = http.GetAsync($"/configuration/{_applicationName}").Result;
      _logger.LogInformation($"{httpResponseMessage.StatusCode}");
    }

    public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs) {
      _applicationName = applicationName;
      _connectionString = connectionString;
      _refreshTimerIntervalInMs = refreshTimerIntervalInMs;
      _logger = NullLogger<ConfigurationReader>.Instance;
      StartConfigurationSyncProcess();
    }

    public async Task<T> GetValueAsync<T>(string key, CancellationToken token) {
      _logger.LogInformation($"{key} requested");
      return default;
    }
  }
}