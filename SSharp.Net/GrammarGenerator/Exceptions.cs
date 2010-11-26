using System;

namespace Scripting.SSharp.Parser
{
  public class GrammarErrorException : Exception
  {
    public GrammarErrorException(string message) : base(message) { }
    public GrammarErrorException(string message, Exception inner) : base(message, inner) { }
  }

  public class CompilerException : Exception
  {
    public CompilerException(string message) : base(message) { }
    public CompilerException(string message, Exception inner) : base(message, inner) { }
  }
}
