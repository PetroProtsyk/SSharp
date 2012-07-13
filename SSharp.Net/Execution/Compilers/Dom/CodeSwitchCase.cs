using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  public class CodeSwitchCase : CodeStatement
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

    public CodeSwitchCase(CodeExpression condition, CodeStatement statement)
    {
      Condition = condition;
      Statement = statement;
    }
  }

}
