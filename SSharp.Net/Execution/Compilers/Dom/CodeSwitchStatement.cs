using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.SSharp.Execution.Compilers.Dom
{
  public class CodeSwitchStatement : CodeStatement
  {
    public CodeExpression Expression
    {
      get;
      private set;
    }

    public List<CodeSwitchCase> Cases
    {
      get;
      private set;
    }

    public CodeSwitchStatement(CodeExpression expression, IEnumerable<CodeSwitchCase> cases)
    {
      Expression = expression;
      Cases = new List<CodeSwitchCase>(cases);
    }
  }

}
