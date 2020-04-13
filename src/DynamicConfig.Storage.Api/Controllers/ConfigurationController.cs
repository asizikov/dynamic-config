using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfig.Storage.DatabaseModel;
using DynamicConfiguration.Storage.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DynamicConfig.Storage.Api.Controllers {
  [ApiController]
  [Route("[controller]")]
  public class ConfigurationController : ControllerBase {
    private readonly ILogger<ConfigurationController> _logger;
    private readonly IDistributedCache _distributedCache;

    public ConfigurationController(ILogger<ConfigurationController> logger, IDistributedCache distributedCache) {
      _logger = logger;
      _distributedCache = distributedCache;
    }

    [HttpGet("{serviceName}")]
    public async Task<ActionResult<ServiceConfigurationResponse>> GetUpdatedStatus(string serviceName, CancellationToken token) {
      _logger.LogInformation($"Received a configuration request for {serviceName}");
      string json;
      try {
        json = await _distributedCache.GetStringAsync(serviceName.ToLowerInvariant(), token).ConfigureAwait(false);
        if (json is null) {
          _logger.LogWarning($"No settings fround for {serviceName}");
          return NotFound();
        }
      }
      catch (Exception e) {
        _logger.LogError(e, $"Failed to read config section for {serviceName}");
        return NotFound();
      }

      try {
        var page = JsonConvert.DeserializeObject<ServiceConfigurationPage>(json);
        var response = new ServiceConfigurationResponse {
          ConfigurationItems = page.Entries
            .Select(c => new ServiceConfigurationItem {
              Type = c.Type,
              Value = c.Value,
              Name = c.Name
            }).ToList()
        };
        return response;
      }
      catch (Exception e) {
        _logger.LogError(e, $"Failed to serialize settings key for {serviceName}.");
        return NotFound();
      }
    }
  }
}