using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace DynamicConfig.Storage.Api.Health {
  // ReSharper disable once ClassNeverInstantiated.Global
  public class DistributedCacheHealthCheck : IHealthCheck {
    private readonly IDistributedCache _distributedCache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DistributedCacheHealthCheck> _logger;

    public DistributedCacheHealthCheck(IDistributedCache distributedCache, IConfiguration configuration, ILogger<DistributedCacheHealthCheck> logger) {
      _distributedCache = distributedCache;
      _configuration = configuration;
      _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
      _logger.LogInformation("DistributedCacheHealthCheck invoked");
      _logger.LogInformation($"redis: {_configuration.GetSection("REDIS").Value}");
      if (_configuration.GetSection("REDIS").Value is null) {
        return Task.FromResult(HealthCheckResult.Unhealthy("Database configuration is missing."));
      }

      if (_distributedCache is null) {
        return Task.FromResult(HealthCheckResult.Unhealthy("Redis service failed to start"));
      }

      return Task.FromResult(HealthCheckResult.Healthy());
    }
  }
}
