using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  public class CodeExpressionStatement : CodeStatement
  {
    public CodeExpression Expression { get; private set; }

    public CodeExpressionStatement(CodeExpression expression)
    {
      Expression = expression;
    }
  }
}
