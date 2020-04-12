using DynamicConfig.Configuration.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DynamicConfig.Configuration.DependencyInjection {
  public static class ServiceCollectionExtensions {
    public static IServiceCollection UseDynamicConfiguration(this IServiceCollection services) {
      services.TryAddSingleton<IConfigurationReader, ConfigurationReader>();
      return services;
    }
  }
}