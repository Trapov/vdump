// unset

namespace Vdump.Api.Exceptions
{
  public sealed class NoFileAddedException : UserFriendlyException {
    public const string Error = "There is nothing to dump. Add file to the payload.";
    public NoFileAddedException() : base(Error)
    {
    }
  }
}
