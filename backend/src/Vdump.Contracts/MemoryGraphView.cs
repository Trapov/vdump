// unset

namespace Vdump.Contracts
{
  using System;
  using System.Collections.Generic;

  public class MemoryGraphView
  {
    public long TotalSize { get; init; }
    public ReportItem[] ReportItems { get; init; }
    public Guid Id { get; init; }
  }
}
