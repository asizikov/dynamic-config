using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfig.Configuration.BackgroundProcess;
using DynamicConfig.Configuration.Cache;
using DynamicConfig.Configuration.StorageClient;
using DynamicConfiguration.Storage.Model;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Xunit;

namespace DynamicConfig.Configuration.Tests.BackgroundProcess {
  public class ConfigurationUpdaterJobTests {
    private readonly CancellationToken _token;
    private readonly IConfigurationUpdaterJob _configurationUpdaterJob;
    private readonly Mock<ISettingsCacheManager> _mockSettingsCacheManager;
    private readonly Mock<IConfigurationStorageClient> _mockConfigurationStorageClient;
    private readonly EmptyCacheMissHandler _handler;

    public ConfigurationUpdaterJobTests() {
      _mockSettingsCacheManager = new Mock<ISettingsCacheManager>();
      _mockConfigurationStorageClient = new Mock<IConfigurationStorageClient>();
      _configurationUpdaterJob = new ConfigurationUpdaterJob(_mockSettingsCacheManager.Object, _mockConfigurationStorageClient.Object,
        NullLogger<ConfigurationUpdaterJob>.Instance);
      _handler = new EmptyCacheMissHandler();
      _token = CancellationToken.None;
    }

    [Fact]
    public void ExecuteAsync_Handles_Client_Failure() {
      _mockConfigurationStorageClient.Setup(c => c.ExecuteAsync(It.IsAny<string>(), _token)).ThrowsAsync(new Exception());
      Should.NotThrow(async () => await _configurationUpdaterJob.ExecuteAsync(It.IsAny<string>(),_token));
      _mockSettingsCacheManager.Verify(
        manager => manager.ReloadSettingsCache(It.IsAny<SettingsCache<int>>(), It.IsAny<SettingsCache<bool>>(), It.IsAny<SettingsCache<string>>()),
        Times.Never());
    }

    [Fact]
    public async Task ExecuteAsync_ReceivedConfig_Updated_Correctly() {
      var config = GetValidConfig();
      _mockConfigurationStorageClient.Setup(client => client.ExecuteAsync(It.IsAny<string>(), _token)).ReturnsAsync(config);
      SettingsCache<int> ints = null;
      SettingsCache<bool> bools = null;
      SettingsCache<string> strings = null;
      _mockSettingsCacheManager.Setup(
        manager => manager.ReloadSettingsCache(It.IsAny<SettingsCache<int>>(), It.IsAny<SettingsCache<bool>>(), It.IsAny<SettingsCache<string>>()))
        .Callback((SettingsCache<int> i, SettingsCache<bool> b, SettingsCache<string> s) => {
          ints = i;
          bools = b;
          strings = s;
        });

      await _configurationUpdaterJob.ExecuteAsync("app", _token);
      _mockSettingsCacheManager.Verify(
        manager => manager.ReloadSettingsCache(It.IsAny<SettingsCache<int>>(), It.IsAny<SettingsCache<bool>>(), It.IsAny<SettingsCache<string>>()),
        Times.Once);

      strings.GetSetting("string-item", _handler).ShouldBe("abcd");
      ints.GetSetting("int-item", _handler).ShouldBe(123);
      bools.GetSetting("bool-item", _handler).ShouldBe(true);
    }

    private static ServiceConfigurationResponse GetValidConfig() =>
      new ServiceConfigurationResponse {
        ConfigurationItems = new List<ServiceConfigurationItem> {
          new ServiceConfigurationItem {
            Name = "string-item",
            Type = ConfigurationItemType.String,
            Value = "abcd"
          },
          new ServiceConfigurationItem{
            Name = "bool-item",
            Type = ConfigurationItemType.Boolean,
            Value = "true"
          },
          new ServiceConfigurationItem{
            Name = "int-item",
            Type = ConfigurationItemType.Integer,
            Value = "123"
          }
        }
      };
  }
}