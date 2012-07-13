using System.Collections.Generic;
using Scripting.SSharp.Parser.FastGrammar;

namespace Scripting.SSharp.Parser
{
  internal class CompilerContext
  {
    public readonly SyntaxErrorList Errors = new SyntaxErrorList();

    public CompilerContext()
    {
    }

    public void AddError(SourceLocation location, string message, ParserState state)
    {
      Errors.Add(new SyntaxError(location, message, state));
    }
  }
}
