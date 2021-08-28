// unset

namespace Vdump.Contracts
{
  public record ErrorResponse
  {
    public string Error { get; init; }
    public string TraceId { get; init; }
    public string ActivityId { get; init; }
  }
}
