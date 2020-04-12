namespace DynamicConfig.Configuration.Cache {
  public interface ISettingsCacheManager {
    void ReloadSettingsCache(SettingsCache<int> integerSettings, SettingsCache<bool> booleanSettings, SettingsCache<string> stringSettings);
  }
}