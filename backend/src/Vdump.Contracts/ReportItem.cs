namespace Vdump.Contracts {
  public record ReportItem {
    public int? Count { get; init; }
    public long SizeBytes { get; init; }
    public string TypeName { get; init; }
    public string ModuleName { get; init; }
  }
}
