using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace DynamicConfig.Database.Tests {
  public class ConfigurationRepositoryTests {
    private readonly ConfigurationRepository _configurationRepository;
    private CancellationToken _token;

    public ConfigurationRepositoryTests() {
      _configurationRepository = new ConfigurationRepository();
      _token = CancellationToken.None;
    }

    [Fact]
    public async Task GetAll_Returns_DatabasePage() {
      var databasePage = await _configurationRepository.GetAllRecordsAsync(_token);
      databasePage.ShouldSatisfyAllConditions(
        () => databasePage.ShouldNotBeNull(),
        () => databasePage.KnownRecords.ShouldNotBeEmpty()
      );
    }
  }
}