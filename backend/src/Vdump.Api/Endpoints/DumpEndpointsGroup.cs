// unset

namespace Vdump.Api.Endpoints {
  using System;
  using System.IO;
  using System.Net;
  using System.Threading.Tasks;

  using Exceptions;

  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Routing;
  using Microsoft.Extensions.Logging;

  using Services;

  public sealed class
    DumpEndpointsGroup : IEndpointGroup {
    public void Configure(IEndpointRouteBuilder app) {
      app.MapPost(
        "api/v1/dumps",
        async (
          ILogger<DumpEndpointsGroup> logger,
          IAnalyzeService analyzeService,
          HttpContext context
        ) => {
          static async Task<(String name, Stream stream)> GetStream(HttpContext context) {
            var form = await context.Request.ReadFormAsync();
            if (form.Files.Count == 0) {
              throw new NoFileAddedException();
            }

            return (form.Files[0].FileName, form.Files[0].OpenReadStream());
          }

          (String fileName, Stream fileReadStream) = await GetStream(context);
          try {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            return Results.Json(
              await analyzeService.From(
                new MemoryDumpRequest
                  (Guid.NewGuid(), fileReadStream, fileName),
                context.RequestAborted)
            );
          }
          catch (Exception ex) {
            logger.LogError(ex, "Something went wrong while reading the dump file");
            throw new FileDumpIsWrongOrCorruptedException();
          }
        }
      );
    }
  }
}
