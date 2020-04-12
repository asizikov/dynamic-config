using System;
using DynamicConfig.Configuration.Abstractions;
using DynamicConfig.Configuration.BackgroundProcess;
using DynamicConfig.Configuration.Cache;
using DynamicConfig.Configuration.StorageClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DynamicConfig.Configuration.DependencyInjection {
  public static class ServiceCollectionExtensions {
    public static IServiceCollection UseDynamicConfiguration(this IServiceCollection services, IConfiguration configuration) {

      services.AddHttpClient<IConfigurationStorageClient,ConfigurationStorageClient>(client => {
        client.BaseAddress = new Uri(configuration[Constants.Env.DYNAMIC_CONFIG_STORAGE_CONNECTION_STRING]);
      });
      services.AddSingleton<IConfigurationUpdaterJob, ConfigurationUpdaterJob>();
      services.AddTransient<IPeriodicJobRunner, PeriodicJobRunner>();
      services.AddTransient<ICacheMissHandler, DefaultCacheMissHandler>(); // look up for a key in `IConfiguration`
      services.TryAddSingleton<IConfigurationReader, AdvancedConfigurationReader>();
      services.AddSingleton<SettingsCacheManager>();
      services.AddSingleton<ISettingsCacheResolver>(p => p.GetRequiredService<SettingsCacheManager>());
      services.AddSingleton<ISettingsCacheManager>(x => x.GetRequiredService<SettingsCacheManager>());
      return services;
    }
  }
}