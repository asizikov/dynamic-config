using DynamicConfiguration.Storage.Model;

namespace DynamicConfig.Storage.DatabaseModel {
  public class ServiceConfigurationActiveRecord {
    public string Name { get; set; }
    public string Value { get; set; }
    public ConfigurationItemType Type { get; set; }
  }
}