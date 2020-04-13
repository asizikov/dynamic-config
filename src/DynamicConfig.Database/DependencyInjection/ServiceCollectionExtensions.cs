using Microsoft.Extensions.DependencyInjection;

namespace DynamicConfig.Database.DependencyInjection {
  public static class ServiceCollectionExtensions {
    public static IServiceCollection UseDatabase(this IServiceCollection services) {
      services.AddTransient<IConfigurationRepository, ConfigurationRepository>();
      services.AddTransient<IStorageAdapter, StorageAdapter>();
      return services;
    }
  }
}