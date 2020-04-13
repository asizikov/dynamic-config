using System;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfig.Database;
using DynamicConfig.Storage.DatabaseModel;
using DynamicConfiguration.Storage.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DatabasePrimer.Service {
  public class Worker : BackgroundService {
    private readonly ILogger<Worker> _logger;
    private readonly IConfigurationRepository _configurationRepository;
    private readonly Random _random;

    public Worker(ILogger<Worker> logger, IConfigurationRepository configurationRepository) {
      _logger = logger;
      _configurationRepository = configurationRepository;
      _random = new Random();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      while (!stoppingToken.IsCancellationRequested) {
        _logger.LogInformation("Worker running at: {time}, will submit new config", DateTimeOffset.Now);
        try {
          await _configurationRepository.InsertAsync(new ServiceConfigurationRecord {
            Type = ConfigurationItemType.String,
            ApplicationName = $"SERVICE-{(_random.Next(10) > 5 ? "A" : "B")}",
            Name = _random.Next(10) > 5 ? "SiteName" : "Connection String",
            Value = $"http://updated-site-name:{DateTime.Now.Ticks}/",
            IsActive = true,
          }, stoppingToken).ConfigureAwait(false);
        }
        catch (Exception e) {
          _logger.LogError(e, "Something went wrong...");
        }

        await Task.Delay(10000, stoppingToken);
      }
    }
  }
}