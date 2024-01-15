using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CipherServices.Services;
using CipherServices.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CipherServices
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var host = CreateHostBuilder(args).Build();
      // This line is commented out to improve performance
      // The initial database should be included in the workspace
      // If it must be re-initialized, delete the current .db file and uncomment this line
      //CreateDbIfNotExists(host);
      host.Run();

    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
          webBuilder.UseStartup<Startup>();
        });

    private static void CreateDbIfNotExists(IHost host)
    {
      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<MessageContext>();
        context.Database.EnsureCreated();
        DbInitializer.Initialize(context);
      }
    }
  }
}
