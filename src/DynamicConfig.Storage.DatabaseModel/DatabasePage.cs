using System.Collections.Generic;

namespace DynamicConfig.Storage.DatabaseModel {
  public class DatabasePage {
    public List<ServiceConfigurationRecord> KnownRecords { get; set; }
  }
}