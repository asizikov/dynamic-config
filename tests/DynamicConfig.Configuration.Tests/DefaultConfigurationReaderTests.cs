using System;
using DynamicConfig.Configuration.Abstractions;
using Shouldly;
using Xunit;

namespace DynamicConfig.Configuration.Tests {
  public class DefaultConfigurationReaderTests {
    private readonly IConfigurationReader _configurationReader;

    public DefaultConfigurationReaderTests() {
      _configurationReader = new ConfigurationReader("unit-tests", "http://connection:123", 1000);
    }

    [Fact]
    public void No_Configuration_Throws() {
      Should.Throw<InvalidOperationException>(() => _configurationReader.GetValue<bool>("key"));
    }
  }
}