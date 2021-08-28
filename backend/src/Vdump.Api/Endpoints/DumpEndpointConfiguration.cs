// unset

namespace Vdump.Api.Endpoints {
  using System;

  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Routing;
  using Microsoft.Extensions.DependencyInjection;

  public sealed partial class
    DumpEndpointConfiguration : IEndpointConfiguration {
    public void Configure(IEndpointRouteBuilder app) {
      app.MapPost(
        "api/v1/dumps",
        c => c.RequestServices.GetRequiredService<Endpoint>().Handle(c)
      );
    }
  }
}
