using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfiguration.Storage.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DynamicConfig.Configuration.StorageClient {
  public class ConfigurationStorageClient : IConfigurationStorageClient {
    private readonly ILogger<ConfigurationStorageClient> _logger;
    private readonly HttpClient _http;

    public ConfigurationStorageClient(ILogger<ConfigurationStorageClient> logger, HttpClient http) {
      _logger = logger;
      _http = http;
    }

    public async Task<ServiceConfigurationResponse> ExecuteAsync(string applicationName, CancellationToken token) {
      var httpResponseMessage = await _http.GetAsync($"/configuration/{applicationName}", token).ConfigureAwait(false);
      _logger.LogInformation($"{httpResponseMessage.StatusCode}");
     
      httpResponseMessage.EnsureSuccessStatusCode();
      
      var responseString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

      return JsonConvert.DeserializeObject<ServiceConfigurationResponse>(responseString);
    }
  }
}