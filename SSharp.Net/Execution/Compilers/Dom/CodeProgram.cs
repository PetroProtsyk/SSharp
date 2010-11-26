using System.Collections.Generic;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  internal class CodeProgram : CodeObject
  {
    private readonly CodeStatements _statements = new CodeStatements();

    public CodeStatements Statements { get { return _statements; } }

    private readonly CodeFunctions _functions = new CodeFunctions();

    public CodeFunctions Functions { get { return _functions; } }
  }

  internal class CodeStatements : List<CodeStatement>
  {
  }

  internal class CodeFunctions : List<CodeObject>
  {
  }

}
