// unset

namespace Vdump.Api.Specs.Dumps {
  using System;
  using System.Net;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading.Tasks;

  using Api.Exceptions;

  using Contracts;

  using FluentAssertions;

  using Microsoft.AspNetCore.Mvc.Testing;

  using Xunit;

  public sealed class ShouldReturnBadRequest : IClassFixture<WebApplicationFactory<Startup>> {
    private readonly WebApplicationFactory<Startup> _applicationFactory;

    public ShouldReturnBadRequest(WebApplicationFactory<Startup> applicationFactory) {
      _applicationFactory = applicationFactory;
    }


    [Fact]
    public async Task WhenNoBodySupplied() {
      var client = _applicationFactory.CreateClient();

      using var mc = new MultipartFormDataContent {
        {
          new StringContent("."), "."
        }
      };
      var response = await client.PostAsync("/api/v1/dumps", mc);

      response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
      var mappedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
      mappedResponse.Should().NotBeNull();
      mappedResponse!.Error.Should().Be(NoFileAddedException.Error);
    }
    
    [Fact]
    public async Task WhenWrongTypeOfFileSupplied() {
      var client = _applicationFactory.CreateClient();

      using var mc = new MultipartFormDataContent {
        {
          new ByteArrayContent(new Byte[] {(byte) "!FastSerialization.1".Length}), "dump", "dump.gcdump"
        }
      };
      var response = await client.PostAsync("/api/v1/dumps", mc);

      response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
      var mappedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
      mappedResponse.Should().NotBeNull();
      mappedResponse!.Error.Should().Be(FileDumpIsWrongOrCorruptedException.Error);
    }
  }
}
