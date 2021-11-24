// unset

namespace Vdump.Api.Services {
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  using Contracts;

  using Exceptions;

  using Graphs;

  using Microsoft.Extensions.Logging;

  using Stores;

  internal sealed class AnalyzeService : IAnalyzeService {
    private readonly ILogger<AnalyzeService> _logger;
    private readonly IStore<MemoryGraphView> _store;

    public AnalyzeService(ILogger<AnalyzeService> logger, IStore<MemoryGraphView> store) {
      _logger = logger;
      _store = store;
    }

    public Task<MemoryGraphView> From(MemoryDumpRequest request,
      CancellationToken cancellationToken) {
      try {
        return _store.GetOrAdd(request.Id, () => {
          var memoryDump = new GCHeapDump(request.Stream, request.FileName);
          return Task.FromResult(new MemoryGraphView {
            Id = request.Id,
            TotalSize = memoryDump.MemoryGraph.TotalSize,
            ReportItems = GetReportItem(memoryDump.MemoryGraph).ToArray()
          });
        });
      }
      catch (Exception ex) {
        _logger.LogError(ex, "Something went wrong while reading the dump file");
        throw new FileDumpIsWrongOrCorruptedException();
      }
    }

    private static IEnumerable<ReportItem> GetReportItem(Graph memoryGraph) {
      var histogramByType = memoryGraph.GetHistogramByType();
      for (var index = 0;
        index < memoryGraph.m_types.Count;
        index++) {
        var type = memoryGraph.m_types[index];
        if (string.IsNullOrEmpty(type.Name) || type.Size == 0)
          continue;

        var sizeAndCount = histogramByType.FirstOrDefault(c => (int)c.TypeIdx == index);
        if (sizeAndCount == null || sizeAndCount.Count == 0)
          continue;

        yield return new ReportItem {
          TypeName = type.Name,
          ModuleName = type.ModuleName,
          SizeBytes = type.Size,
          Count = sizeAndCount.Count
        };
      }
    }
  }
}
