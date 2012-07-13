using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  public class CodeWhileStatement : CodeStatement
  {
    public CodeExpression Condition
    {
      get;
      private set;
    }

    public CodeStatement Statement
    {
      get;
      private set;
    }

    public CodeWhileStatement(CodeExpression condition, CodeStatement statement)
    {
      Condition = condition;
      Statement = statement;
    }
  }

}
