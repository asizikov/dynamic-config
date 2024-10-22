using DynamicConfig.Database.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace DatabasePrimer.Service {
  public class Program {
    public static void Main(string[] args) {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) => {
          services.AddSingleton(new RedisConfiguration {
            Hosts = new[] {
              new RedisHost {Host = hostContext.Configuration.GetSection("REDIS").Value, Port = 6379},
            }
          }
          );
          services.AddStackExchangeRedisCache(options => { options.Configuration = hostContext.Configuration.GetSection("REDIS").Value; });
          services.UseDatabase();
          services.AddSingleton<IRedisClient, RedisClient>();
          services.AddSingleton<IRedisConnectionPoolManager, RedisConnectionPoolManager>();
          services.AddSingleton<ISerializer, NewtonsoftSerializer>();
          services.AddHostedService<Worker>();
        });
  }
}