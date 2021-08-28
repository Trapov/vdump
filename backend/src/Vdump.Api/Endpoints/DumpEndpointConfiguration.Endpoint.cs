// unset

namespace Vdump.Api.Endpoints {
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Threading.Tasks;

  using Contracts;

  using Exceptions;

  using Graphs;

  using Microsoft.AspNetCore.Http;
  using Microsoft.Extensions.Caching.Memory;
  using Microsoft.Extensions.Logging;

  using Services;

  public sealed partial class DumpEndpointConfiguration {
    internal sealed class Endpoint : IEndpoint {
      private readonly ILogger<Endpoint> _logger;
      private readonly IAnalyzeService _analyzeService;

      public Endpoint(ILogger<Endpoint> logger, IAnalyzeService analyzeService) {
        _logger = logger;
        _analyzeService = analyzeService;
      }

      public async Task Handle(HttpContext context) {
        var form = await context.Request.ReadFormAsync();
        if (form.Files.Count == 0) {
          throw new NoFileAddedException();
        }

        var file = form.Files[0];

        var fileName = file.FileName;
        var fileReadStream = file.OpenReadStream();

        try {
          context.Response.StatusCode = (int)HttpStatusCode.OK;
          await context.Response.WriteAsJsonAsync(await _analyzeService.GetOrAdd(Guid.NewGuid(), fileReadStream, fileName, context.RequestAborted));
        }
        catch (Exception ex) {
          _logger.LogError(ex, "Something went wrong while reading the dump file");
          throw new FileDumpIsWrongOrCorruptedException();
        }
      }
    }
  }
}
