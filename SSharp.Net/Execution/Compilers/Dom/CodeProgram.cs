using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  public class CodeProgram : CodeObject
  {
    private readonly CodeStatements statements = new CodeStatements();

    public CodeStatements Statements { get { return statements; } }

    private readonly CodeFunctions functions = new CodeFunctions();

    public CodeFunctions Functions { get { return Functions; } }
  }

  public class CodeStatements : List<CodeStatement>
  {
  }

  public class CodeFunctions : List<CodeObject>
  {
  }

}
