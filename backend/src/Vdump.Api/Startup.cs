using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Vdump.Api {
  using System.Diagnostics;
  using System.IO;

  using Contracts;

  using Endpoints;

  using Exceptions;

  using Microsoft.AspNetCore.Diagnostics;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Http.Extensions;
  using Microsoft.AspNetCore.Http.Features;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;

  using Serilog;

  using Services;

  public class Startup {
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services) {
      services.AddCors().AddMemoryCache();
      services.AddTransient<IAnalyzeService, AnalyzeService.Cache>();
      services.AddSingleton<AnalyzeService>();
      services.Scan(x =>
        x.FromAssemblyOf<Startup>()
          .AddClasses(f => f.AssignableTo<IEndpointConfiguration>()).As<IEndpointConfiguration>()
          .WithSingletonLifetime()
          .AddClasses(f => f.AssignableTo<IEndpoint>()).AsSelf().WithTransientLifetime()
      );

      services.Configure<FormOptions>(x => {
        x.ValueLengthLimit = int.MaxValue;
        x.MultipartBodyLengthLimit = int.MaxValue;
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseCors(x => x.AllowAnyOrigin());
      }

      app.UseRouting();

      app.UseSerilogRequestLogging(options => {
        options.EnrichDiagnosticContext = (context, httpContext) => {
          var endpoint = httpContext.GetEndpoint();
          context.Set("AbsoluteUrl", httpContext.Request.GetDisplayUrl());
          context.Set("Endpoint", endpoint?.DisplayName);
        };
      });

      app.UseExceptionHandler(new ExceptionHandlerOptions {
        ExceptionHandler = async x => {
          using var scope = x.RequestServices.CreateScope();
          var context = x.Features.Get<IExceptionHandlerFeature>();

          var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();

          logger.LogError(
            context.Error, "An error occured while executing an endpoint"
          );

          if (context.Error is UserFriendlyException ufe) {
            x.Response.StatusCode = StatusCodes.Status400BadRequest;
            await x.Response.WriteAsJsonAsync(new ErrorResponse {
              Error = ufe.Message,
              TraceId = x.TraceIdentifier,
              ActivityId = Activity.Current?.Id
            });
            return;
          }

          x.Response.StatusCode = StatusCodes.Status500InternalServerError;
          await x.Response.WriteAsJsonAsync(new ErrorResponse {
            Error = "Unknown error",
            TraceId = x.TraceIdentifier,
            ActivityId = Activity.Current?.Id
          });
        },
      });


      app.UseEndpoints(builder => {
        foreach (var configure in app.ApplicationServices.GetServices<IEndpointConfiguration>()) {
          configure.Configure(builder);
        }
      });
    }
  }
}
