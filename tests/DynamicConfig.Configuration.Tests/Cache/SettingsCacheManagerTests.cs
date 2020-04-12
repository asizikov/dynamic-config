using System;
using DynamicConfig.Configuration.Cache;
using Shouldly;
using Xunit;

namespace DynamicConfig.Configuration.Tests.Cache {
  public class SettingsCacheManagerTests {
    private readonly SettingsCacheManager _settingsCacheManager;

    public SettingsCacheManagerTests() {
      _settingsCacheManager = new SettingsCacheManager();
    }

    [Fact]
    public void ColdStart_SettingsCache_Can_Be_Resoled() {
      _settingsCacheManager.ShouldSatisfyAllConditions(
        () => _settingsCacheManager.ResolveSettingCache<bool>().ShouldNotBeNull(),
        () => _settingsCacheManager.ResolveSettingCache<string>().ShouldNotBeNull(),
        () => _settingsCacheManager.ResolveSettingCache<int>().ShouldNotBeNull()
      );
    }

    [Fact]
    public void Unknown_Setting_Type_Should_Throw() {
      Should.Throw<InvalidOperationException>(() => _settingsCacheManager.ResolveSettingCache<double>());
    }

    [Fact]
    public void After_Reload_SettingsCache_Replaced() {
      var bools = _settingsCacheManager.ResolveSettingCache<bool>();
      var strings = _settingsCacheManager.ResolveSettingCache<string>();
      var ints = _settingsCacheManager.ResolveSettingCache<int>();

      _settingsCacheManager.ReloadSettingsCache(SettingsCache<int>.Empty, SettingsCache<bool>.Empty, SettingsCache<string>.Empty);

      _settingsCacheManager.ShouldSatisfyAllConditions(
        () => _settingsCacheManager.ResolveSettingCache<bool>().ShouldNotBeSameAs(bools),
        () => _settingsCacheManager.ResolveSettingCache<string>().ShouldNotBeSameAs(strings),
        () => _settingsCacheManager.ResolveSettingCache<int>().ShouldNotBeSameAs(ints));
    }
  }
}