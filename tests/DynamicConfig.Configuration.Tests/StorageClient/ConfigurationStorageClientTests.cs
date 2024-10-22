using System;
using DynamicConfig.Configuration.StorageClient;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DynamicConfig.Configuration.Tests.StorageClient {
  public class ConfigurationStorageClientTests {
    private readonly IConfigurationStorageClient _storageClient;

    public ConfigurationStorageClientTests() {
      _storageClient = new ConfigurationStorageClient(NullLogger<ConfigurationStorageClient>.Instance, null);
    }

    [Fact]
    public void Test1() {
    }
  }
}