using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  public class CodeIfStatement : CodeStatement
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

    public CodeStatement ElseStatement
    {
      get;
      private set;
    }

    public CodeIfStatement(CodeExpression condition, CodeStatement statement, CodeStatement elseStatement)
    {
      Condition = condition;
      Statement = statement;
      ElseStatement = elseStatement;
    }
  }

}
