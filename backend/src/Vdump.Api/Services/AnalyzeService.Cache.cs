// unset

namespace Vdump.Api.Services
{
  using System;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;

  using Contracts;

  using Exceptions;

  using Microsoft.Extensions.Caching.Memory;

  internal sealed partial class AnalyzeService
  {
    internal sealed class Cache : IAnalyzeService {
      private readonly IMemoryCache _memory;
      private readonly AnalyzeService _analyzeService;

      public Cache(IMemoryCache memory, AnalyzeService analyzeService) {
        _memory = memory;
        _analyzeService = analyzeService;
      }


      public async Task<MemoryGraphView> GetOrAdd(Guid id, Stream stream, string fileStream, CancellationToken cancellationToken) {
        var response = await _memory.GetOrCreateAsync(id, entry => stream is null ? throw new SuchDumpWasNotFoundException() : _analyzeService.Analyze(stream, fileStream, cancellationToken));
        return _memory.Set(id, response, TimeSpan.FromDays(1));
      }
    }
  }
}
