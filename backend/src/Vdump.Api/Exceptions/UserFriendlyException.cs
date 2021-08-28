// unset

namespace Vdump.Api.Exceptions
{
  using System;

  public abstract class UserFriendlyException : Exception {
    public UserFriendlyException(string message) : base(message) {
      
    }
  }

  public sealed class SuchDumpWasNotFoundException : UserFriendlyException {
    public SuchDumpWasNotFoundException() : base("Such dump was not found.") {
      
    }
  }
}
