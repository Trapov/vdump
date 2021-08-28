using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Vdump.Api {
  using Serilog;

  public class Program {
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .UseSerilog(
          (x, c) => c.ReadFrom.Configuration(x.Configuration)
        )
        .ConfigureWebHostDefaults(webBuilder => {
          webBuilder.UseStartup<Startup>().UseKestrel(options =>
          {
            options.Limits.MaxRequestBodySize = 1 * 1024 * 1024 * 200;
          });;
        });
  }
}
