// unset

namespace Vdump.Api.Endpoints
{
  using System.Threading.Tasks;

  using Microsoft.AspNetCore.Http;

  public interface IEndpoint {
    public Task Handle(HttpContext context);
  }
}
