using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DynamicConfig.Configuration.BackgroundProcess {
  public interface IPeriodicJobRunner {
    void Start();
  }

  public class PeriodicJobRunner : IPeriodicJobRunner, IDisposable {
    private readonly ILogger<PeriodicJobRunner> _logger;
    private readonly IConfigurationUpdaterJob _job;
    private readonly string _applicationName;
    private readonly int _refreshInterval;
    private Timer _timer;
    private CancellationTokenSource _cancellationTokenSource;

    public PeriodicJobRunner(ILogger<PeriodicJobRunner> logger, IConfigurationUpdaterJob job, IConfiguration configuration)
      : this(logger, job, configuration.GetSection(Constants.Env.DYNAMIC_CONFIG_APPLICATION_NAME).Value,
        int.Parse(configuration.GetSection(Constants.Env.DYNAMIC_CONFIG_REFRESH_INTERVAL_MS).Value)) {
    }

    public PeriodicJobRunner(ILogger<PeriodicJobRunner> logger, IConfigurationUpdaterJob job, string applicationName, int refreshInterval) {
      _logger = logger;
      _job = job;
      _applicationName = applicationName;
      _refreshInterval = refreshInterval;
      _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Start() {
      _timer = new Timer(ExecuteJob, null, TimeSpan.Zero,
        TimeSpan.FromMilliseconds(_refreshInterval));
    }

    private void ExecuteJob(object state) {
      Task.Run(() =>
        _job.ExecuteAsync(_applicationName, _cancellationTokenSource.Token)).GetAwaiter().GetResult();
    }

    public void Dispose() {
      _timer?.Dispose();
    }
  }
}