// unset

namespace Vdump.Api.Exceptions
{
  public sealed class FileDumpIsWrongOrCorruptedException : UserFriendlyException {
    public const string Error = "File dump has a wrong type that we could not parse. Try another one.";
    public FileDumpIsWrongOrCorruptedException() : base(Error)
    {
    }
  }
}
