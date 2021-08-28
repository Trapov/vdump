// unset

namespace Vdump.Api.Endpoints
{
  using Microsoft.AspNetCore.Routing;

  public interface IEndpointConfiguration {
    public void Configure(IEndpointRouteBuilder app);
  }
}
