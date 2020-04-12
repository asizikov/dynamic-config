using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfig.Storage.DatabaseModel;
using DynamicConfiguration.Storage.Model;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DatabasePrimer.Service {
  public class Worker : BackgroundService {
    private readonly ILogger<Worker> _logger;
    private readonly IDistributedCache _distributedCache;

    public Worker(ILogger<Worker> logger, IDistributedCache distributedCache) {
      _logger = logger;
      _distributedCache = distributedCache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      while (!stoppingToken.IsCancellationRequested) {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        try {
          var json = JsonConvert.SerializeObject(
            new List<ServiceConfigurationRecord> {
              new ServiceConfigurationRecord {
                Type = ConfigurationItemType.String,
                Name = "SiteName",
                IsActive = true,
                Value = $"http://updated-site-name:{DateTime.Now.Ticks}/"
              },
              new ServiceConfigurationRecord {
                IsActive = false,
                Type = ConfigurationItemType.String,
                Name = "SiteName2",
                Value = $"http://inactive-site-name:{DateTime.Now.Ticks}/"
              }
            }
          );
          await _distributedCache.SetStringAsync("service-a", json, stoppingToken);
        }
        catch (Exception e) {
          _logger.LogError(e, "Something went wrong...");
        }

        await Task.Delay(15000, stoppingToken);
      }
    }
  }
}