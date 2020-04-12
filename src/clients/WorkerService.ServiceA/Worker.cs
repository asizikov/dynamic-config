using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfig.Configuration.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerService.ServiceA {
  public class Worker : BackgroundService {
    private readonly ILogger<Worker> _logger;
    private readonly IConfigurationReader _configurationReader;

    public Worker(ILogger<Worker> logger, IConfigurationReader configurationReader) {
      _logger = logger;
      _configurationReader = configurationReader;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      while (!stoppingToken.IsCancellationRequested) {
        try {
          _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
          var value = _configurationReader.GetValue<string>("SiteName");
          _logger.LogInformation($"Loaded value {value} for the settings key 'SiteName'");
          await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
        }
        catch (Exception e) {
          _logger.LogError(e, "Well, we'll keep working...");
        }
      }
    }
  }
}