using System;

namespace Scripting.SSharp
{
  public class GrammarErrorException : Exception
  {
    public GrammarErrorException(string message) : base(message) { }
    public GrammarErrorException(string message, Exception inner) : base(message, inner) { }
  }
}
