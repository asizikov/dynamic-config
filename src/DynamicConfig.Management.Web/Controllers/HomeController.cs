using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfig.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DynamicConfig.Management.Web.Models;

namespace DynamicConfig.Management.Web.Controllers {
  public class HomeController : Controller {
    private readonly ILogger<HomeController> _logger;
    private readonly IConfigurationRepository _configurationRepository;

    public HomeController(ILogger<HomeController> logger, IConfigurationRepository configurationRepository) {
      _logger = logger;
      _configurationRepository = configurationRepository;
    }

    public async Task<IActionResult> Index(CancellationToken token) {
      _logger.LogInformation("Index page requested");
      var databasePage = await _configurationRepository.GetAllRecordsAsync(token);
      _logger.LogInformation($"Loaded {databasePage.KnownRecords.Count} items");
      return View(new DatabasePageViewModel {
        Records = databasePage.KnownRecords.Select(s => new ServiceConfigurationRecordViewModel {
          Id = s.Id,
          Type = s.Type.ToString(),
          ApplicationName = s.ApplicationName,
          IsActive = s.IsActive,
          Name = s.Name,
          Value = s.Value
        }).ToList()
      });
    }

    public async Task<IActionResult> ChangeState(Guid id, CancellationToken token) {
      await _configurationRepository.InvertStateAsync(id, token);
      return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id, CancellationToken token) {
      await _configurationRepository.DeleteAsync(id, token);
      return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy() {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
      return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
  }
}