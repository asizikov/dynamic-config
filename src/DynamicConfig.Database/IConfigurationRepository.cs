using System;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfig.Storage.DatabaseModel;

namespace DynamicConfig.Database {
  public interface IConfigurationRepository {
    Task<DatabasePage> GetAllRecordsAsync(CancellationToken token);
    Task InvertStateAsync(Guid id, in CancellationToken token);
    Task DeleteAsync(Guid id, CancellationToken token);
    Task InsertAsync(ServiceConfigurationRecord record, CancellationToken token);
  }
}