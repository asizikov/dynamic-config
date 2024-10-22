using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicConfig.Database;
using DynamicConfig.Database.DependencyInjection;
using Lamar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace DynamicConfig.Management.Web {
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddControllersWithViews();
    }

    public void ConfigureContainer(ServiceRegistry services) {
      services.Scan(s => {
        s.TheCallingAssembly();
        s.WithDefaultConventions();
      });
      services.Scan(s => {
        s.Assembly(typeof(IConfigurationRepository).Assembly);
        s.WithDefaultConventions();
      });
      services.AddSingleton(new RedisConfiguration {
        Hosts = new[] {
          new RedisHost {Host = Configuration.GetSection("REDIS").Value, Port = 6379},
        }
      });
      services.AddStackExchangeRedisCache(options => { options.Configuration = Configuration.GetSection("REDIS").Value; });
      services.AddSingleton<IRedisClient, RedisClient>();
      services.AddSingleton<IRedisConnectionPoolManager, RedisConnectionPoolManager>();
      services.AddSingleton<ISerializer, NewtonsoftSerializer>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      }
      else {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints => {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}