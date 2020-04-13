using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using StackExchange.Redis.Extensions.Core.Abstractions;
using Xunit;

namespace DynamicConfig.Database.Tests {
  public class ConfigurationRepositoryTests {
    private readonly ConfigurationRepository _configurationRepository;
    private readonly Mock<IRedisCacheClient> _mockRedisCacheClient;
    private CancellationToken _token;

    public ConfigurationRepositoryTests() {
      _mockRedisCacheClient = new Mock<IRedisCacheClient>();
      _mockRedisCacheClient.Setup(redis => redis.Db0).Returns(new Mock<IRedisDatabase>().Object);
      _configurationRepository = new ConfigurationRepository(_mockRedisCacheClient.Object, NullLogger<ConfigurationRepository>.Instance);
      _token = CancellationToken.None;
    }

    [Fact]
    public async Task GetAll_Returns_DatabasePage() {
      var databasePage = await _configurationRepository.GetAllRecordsAsync(_token);
      databasePage.ShouldSatisfyAllConditions(
        () => databasePage.ShouldNotBeNull(),
        () => databasePage.KnownRecords.ShouldNotBeNull()
      );
    }
  }
}