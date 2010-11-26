using System.Collections.Generic;

namespace Scripting.SSharp.Parser
{
  internal class SyntaxError
  {
    public SyntaxError(SourceLocation location, string message, ParserState state)
    {
      Location = location;
      Message = message;
      State = state;
    }

    public readonly SourceLocation Location;
    public readonly string Message;
    public readonly ParserState State;
  }
}
