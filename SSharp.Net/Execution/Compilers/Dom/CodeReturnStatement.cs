using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  public class CodeReturnStatement : CodeStatement
  {
    public CodeExpression Expression
    {
      get;
      private set;
    }

    public CodeReturnStatement(CodeExpression expression)
    {
      Expression = expression;
    }
  }

}
