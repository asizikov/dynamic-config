using System;
using System.Collections.Generic;
using Xunit;
using DynamicConfig.Configuration.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace DynamicConfig.Configuration.Cache.Tests {
    public class DefaultCacheMissHandlerTests {
        private readonly ICacheMissHandler _cacheMissHandler;

        [Fact]
        public void Constructuor_NullLogger_Throws() {
            
            Assert.Throws<ArgumentNullException>(() => new DefaultCacheMissHandler(null, null));
        }

        [Fact]
        public void Constructor_NullConfiguration_Throws() {

            Assert.Throws<ArgumentNullException>(() => new DefaultCacheMissHandler(NullLogger<DefaultCacheMissHandler>.Instance, null));
        }

        [Fact]
        public void Get_KnownKey_ReturnsValue() {

            var inMemoryConfigSettings = new Dictionary<string, string> {
                {"key1", "value1"},
                {"key2", "value2"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfigSettings)
                .Build();   

            var handler = new DefaultCacheMissHandler(NullLogger<DefaultCacheMissHandler>.Instance, configuration);

            var result = handler.HandleCacheMiss<string>("key1");
            Assert.Equal("value1", result);            
        }
    }    
}
