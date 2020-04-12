namespace DynamicConfig.Configuration.Cache {
  public interface ISettingsCacheResolver {
    SettingsCache<T> ResolveSettingCache<T>();
  }
}