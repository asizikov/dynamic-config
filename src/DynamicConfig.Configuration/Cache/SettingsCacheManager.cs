using System;

namespace DynamicConfig.Configuration.Cache {
  public class SettingsCacheManager : ISettingsCacheResolver, ISettingsCacheManager {
    private readonly object _cacheLock = new object();

    private SettingsCache<int> _integerSettings;
    private SettingsCache<bool> _booleanSettings;
    private SettingsCache<string> _stringSettings;

    public SettingsCacheManager() {
      _integerSettings = SettingsCache<int>.Empty;
      _booleanSettings = SettingsCache<bool>.Empty;
      _stringSettings = SettingsCache<string>.Empty;
    }

    public SettingsCache<T> ResolveSettingCache<T>() {
      lock (_cacheLock) {
        var type = typeof(T);
        if (type == typeof(string)) {
          return _stringSettings as SettingsCache<T>;
        }

        if (type == typeof(bool)) {
          return _booleanSettings as SettingsCache<T>;
        }

        if (type == typeof(int)) {
          return _integerSettings as SettingsCache<T>;
        }

        throw new InvalidOperationException($"Settings of type {typeof(T)} are not supported");
      }
    }

    public void ReloadSettingsCache(SettingsCache<int> integerSettings, SettingsCache<bool> booleanSettings, SettingsCache<string> stringSettings) {
      lock (_cacheLock) {
        _booleanSettings = booleanSettings;
        _integerSettings = integerSettings;
        _stringSettings = stringSettings;
      }
    }
  }
}