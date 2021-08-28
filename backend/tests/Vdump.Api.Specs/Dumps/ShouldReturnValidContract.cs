// unset

namespace Vdump.Api.Specs.Dumps
{
  using System.IO;
  using System.Net;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Text.Json;
  using System.Threading.Tasks;

  using Contracts;

  using FluentAssertions;

  using Microsoft.AspNetCore.Mvc.Testing;

  using Xunit;

  public sealed class ShouldReturnValidContract : IClassFixture<WebApplicationFactory<Startup>> {
    private readonly WebApplicationFactory<Startup> _applicationFactory;

    public ShouldReturnValidContract(WebApplicationFactory<Startup> applicationFactory) {
      _applicationFactory = applicationFactory;
    }


    [Fact]
    public async Task WhenValidFileSupplied() {
      var client = _applicationFactory.CreateClient();

      var mc = new MultipartFormDataContent {
        {new StreamContent(File.OpenRead("./Assets/example.gcdump")), "example", "example.gcdump"}
      };

      var response = await client.PostAsync("/api/v1/dumps", mc);
      response.StatusCode.Should().Be(HttpStatusCode.OK);

      var mappedResponse = await response.Content.ReadFromJsonAsync<MemoryGraphView>(new JsonSerializerOptions {
        PropertyNameCaseInsensitive = true,
      });
      mappedResponse.Should().NotBeNull();
      mappedResponse!.ReportItems.Should().HaveCountGreaterThan(0);
      mappedResponse!.TotalSize.Should().BeGreaterThan(0);
    }
  }
}
