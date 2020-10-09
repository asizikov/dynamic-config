using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace DynamicConfig.Storage.Api.Health {
  // ReSharper disable once ClassNeverInstantiated.Global
  public class HealthCheck : IHealthCheck {
    private readonly IDistributedCache _distributedCache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HealthCheck> _logger;

    public HealthCheck(IDistributedCache distributedCache, IConfiguration configuration, ILogger<HealthCheck> logger) {
      _distributedCache = distributedCache;
      _configuration = configuration;
      _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
      _logger.LogInformation("HealthCheck invoked");
      _logger.LogInformation($"redis: {_configuration.GetSection("REDIS").Value}");
      if (_configuration.GetSection("REDIS").Value is null) {
        return HealthCheckResult.Unhealthy("Database configuration is missing.");
      }

      if (_distributedCache is null) {
        return HealthCheckResult.Unhealthy("Redis service failed to start");
      }

      return HealthCheckResult.Healthy();
    }
  }
}
