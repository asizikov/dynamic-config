using System;
using DynamicConfiguration.Storage.Model;

namespace DynamicConfig.Storage.DatabaseModel {
  public class ServiceConfigurationRecord {
    public Guid Id { get; set; }
    public string Value { get; set; }
    public string Name { get; set; }
    public ConfigurationItemType Type { get; set; }
    public bool IsActive { get; set; }
    public string ApplicationName { get; set; }
  }
}