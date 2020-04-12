namespace DynamicConfig.Configuration.Cache {
  public interface ICacheMissHandler {
    TType HandleCacheMiss<TType>(string key);
  }
}