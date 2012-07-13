using System.Collections.Generic;

namespace Scripting.SSharp.Parser
{
  internal class CompilerContext
  {
    public readonly LanguageCompiler Compiler;
    public readonly SyntaxErrorList Errors = new SyntaxErrorList();

    public CompilerContext(LanguageCompiler compiler)
    {
      this.Compiler = compiler;
    }

    public void AddError(SourceLocation location, string message, ParserState state)
    {
      Errors.Add(new SyntaxError(location, message, state));
    }
  }
}
