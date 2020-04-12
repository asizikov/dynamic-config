using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamicConfig.Storage.Api.Controllers {
  [ApiController]
  [Route("[controller]")]
  public class ConfigurationController : ControllerBase {
    private readonly ILogger<ConfigurationController> _logger;

    public ConfigurationController(ILogger<ConfigurationController> logger) {
      _logger = logger;
    }

    [HttpGet("{serviceName}")]
    public async Task<ActionResult<ServiceConfiguration>> GetUpdatedStatus(string serviceName, CancellationToken token) {
      _logger.LogInformation($"Received a configuration request for {serviceName}");
      return new ServiceConfiguration();
    }
  }
}