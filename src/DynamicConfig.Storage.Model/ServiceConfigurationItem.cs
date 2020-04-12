namespace DynamicConfiguration.Storage.Model {
  public class ServiceConfigurationItem {
    public string Value { get; set; }
    public string Name { get; set; }
    public ConfigurationItemType Type { get; set; }
  }
}