using System.Threading;
using System.Threading.Tasks;

namespace DynamicConfig.Configuration.BackgroundProcess {
  public interface IConfigurationUpdaterJob {
    Task ExecuteAsync(string applicationName, CancellationToken token);
  }
}