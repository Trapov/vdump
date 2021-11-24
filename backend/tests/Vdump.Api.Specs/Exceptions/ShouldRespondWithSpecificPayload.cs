// unset

namespace Vdump.Api.Specs.Exceptions
{
  using System;
  using System.Net;
  using System.Net.Http.Json;
  using System.Threading.Tasks;

  using Api.Exceptions;

  using Contracts;

  using Endpoints;

  using FluentAssertions;

  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Microsoft.AspNetCore.Routing;
  using Microsoft.Extensions.DependencyInjection;

  using Xunit;

  public sealed class ShouldRespondWithSpecificPayload : IClassFixture<WebApplicationFactory<Startup>> {
    private readonly WebApplicationFactory<Startup> _applicationFactory;

    public ShouldRespondWithSpecificPayload(WebApplicationFactory<Startup> applicationFactory) {
      _applicationFactory = applicationFactory;
    }

    private sealed class UserNotFoundException : UserFriendlyException {
      public UserNotFoundException(string message) : base(message) {
      }
    }

    private sealed class ErrorTest : IEndpointGroup {
      private readonly string _pattern;
      private readonly Action _throwEx;

      public ErrorTest(string pattern, Action throwEx) {
        _pattern = pattern;
        _throwEx = throwEx;
      }

      public void Configure(IEndpointRouteBuilder app) {
        app.MapGet(_pattern, async _ => _throwEx());
      }
    }

    [Fact]
    public async Task WhenGeneralErrorOccured() {
      var factory = _applicationFactory.WithWebHostBuilder(x => x.ConfigureServices(s => {
        s.AddSingleton<IEndpointGroup>(new ErrorTest("/test", () => throw new Exception()));
      }));
      var client = factory.CreateClient();
      var response = await client.GetAsync("/test");

      response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
      var mappedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

      mappedResponse.Should().NotBeNull();
      mappedResponse!.Error.Should().Be("Unknown error");
    }

    [Fact]
    public async Task WhenUserFriendlyExceptionOccured() {
      var factory = _applicationFactory.WithWebHostBuilder(x => x.ConfigureServices(s => {
        s.AddSingleton<IEndpointGroup>(new ErrorTest("/test",
          () => throw new UserNotFoundException("User not found")));
      }));
      var client = factory.CreateClient();
      var response = await client.GetAsync("/test");

      response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
      var mappedResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

      mappedResponse.Should().NotBeNull();
      mappedResponse!.Error.Should().Be("User not found");
    }
  }
}
