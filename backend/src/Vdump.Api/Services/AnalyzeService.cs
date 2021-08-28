// unset

namespace Vdump.Api.Services {
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Threading;
  using System.Threading.Tasks;

  using Contracts;

  using Exceptions;

  using Graphs;

  using Microsoft.Diagnostics.Tracing.AutomatedAnalysis;
  using Microsoft.Extensions.Logging;

  internal sealed partial class AnalyzeService {
    private readonly ILogger<AnalyzeService> _logger;

    public AnalyzeService(ILogger<AnalyzeService> logger) {
      _logger = logger;
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
    
    public async Task<MemoryGraphView> Analyze(Stream stream, string fileName, CancellationToken cancellationToken) {
      try
      {
        var memoryDump = new GCHeapDump(stream, fileName);

        return new MemoryGraphView
        {
          Id = Guid.NewGuid(),
          TotalSize = memoryDump.MemoryGraph.TotalSize,
          ReportItems = GetReportItem(memoryDump.MemoryGraph).ToArray()
        };
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Something went wrong while reading the dump file");
        throw new FileDumpIsWrongOrCorruptedException();
      }
    }
  }

  public interface IAnalyzeService {
    Task<MemoryGraphView> GetOrAdd(Guid id, Stream stream, string fileStream, CancellationToken cancellationToken);
  }
  
}
