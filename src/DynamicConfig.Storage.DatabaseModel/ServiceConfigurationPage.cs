using System.Collections.Generic;

namespace DynamicConfig.Storage.DatabaseModel {
  public class ServiceConfigurationPage {
    public List<ServiceConfigurationActiveRecord> Entries { get; set; }
  }
}