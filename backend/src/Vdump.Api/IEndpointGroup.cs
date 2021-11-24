// unset

namespace Vdump.Api
{
  using Microsoft.AspNetCore.Routing;

  public interface IEndpointGroup {
    public void Configure(IEndpointRouteBuilder app);
  }
}
