namespace DynamicConfig.Configuration.Abstractions {
  public interface IConfigurationReader {
    T GetValue<T>(string key);
  }
}