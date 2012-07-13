using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  public class CodeBlockStatement : CodeStatement
  {
    public CodeStatements Statements
    {
      get;
      private set;
    }

    public CodeBlockStatement()
    {
      Statements = new CodeStatements();
    }
  }

}
