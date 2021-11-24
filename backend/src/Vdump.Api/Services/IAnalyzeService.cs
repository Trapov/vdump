namespace Vdump.Api.Services
{
  using System;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;

  using Contracts;

  public readonly record struct MemoryDumpRequest(Guid Id, Stream Stream, string FileName);
  public interface IAnalyzeService {
    Task<MemoryGraphView> From(MemoryDumpRequest request, CancellationToken cancellationToken);
  }
}
