using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DynamicConfig.Configuration.Cache {
  public sealed class SettingsCache<TType> {
    private readonly IReadOnlyDictionary<string, Setting<TType>> _cache;

    public SettingsCache(IReadOnlyDictionary<string, Setting<TType>> cache) {
      _cache = cache;
    }

    public TType GetSetting(string key, ICacheMissHandler cacheMissHandler) {
      if (_cache.ContainsKey(key)) {
        return _cache[key].Value;
      }

      return cacheMissHandler.HandleCacheMiss<TType>(key);
    }

    public static SettingsCache<TType> Empty =>
      new SettingsCache<TType>(new ReadOnlyDictionary<string, Setting<TType>>(new Dictionary<string, Setting<TType>>()));
  }
}