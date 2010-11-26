using System;

namespace Scripting.SSharp
{
  public class CompilerException : Exception
  {
    public CompilerException(string message) : base(message) { }
    public CompilerException(string message, Exception inner) : base(message, inner) { }
  }
}
