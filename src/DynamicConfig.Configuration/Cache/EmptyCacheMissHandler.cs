using System;

namespace DynamicConfig.Configuration.Cache {
  public class EmptyCacheMissHandler : ICacheMissHandler {
    public TType HandleCacheMiss<TType>(string key) {
      throw new InvalidOperationException($"Unknown config key {key} requested for type {typeof(TType)}");
    }
  }
}